using Core;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ClosedXML.Excel;

namespace Server
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        
        private IUserRepository _userRepository;
        private IHotelRepository _hotelRepository;
        
        private static Dictionary<string, (string Kode, DateTime Expiry)> verificeringsKoder = new();


        public UserController(IUserRepository userRepository, IHotelRepository hotelRepository)
        {
            _userRepository = userRepository;
            _hotelRepository = hotelRepository;
        }

    
        /// <summary>
        /// Henter alle bruger og konverter til dto
        /// </summary>
        /// <returns>Retunerer en liste af users</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUsers = await _userRepository.GetAllUsers();
            
            List<BrugerLoginDTO> brugerLogins = new();

            foreach (User user in allUsers)
            {
                brugerLogins.Add(new BrugerLoginDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Password = user.Password,
                    Rolle = user.Rolle,
                    FirstName = user.FirstName,
                    HotelId = user.HotelId
                });
            }
            return Ok(brugerLogins);
        }

        /// <summary>
        /// Henter alle bruger uden mig selv
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retunerer alle users uden mig selv</returns>
        [HttpGet]
        [Route("withoutmyself/{id:int}")]
        public async Task<IActionResult> GetAllUsersWithOutMyself(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id er forkert");
            }
            
            List<BrugerAdministrationDTO> brugers = new();

            var allUsers = await _userRepository.GetAllUsersWithOutMyself(id);


            foreach (var x in allUsers)
            {
                brugers.Add(new BrugerAdministrationDTO
                {
                    Id = x.Id,
                    FirstName = x.FirstName + " " + x.LastName,
                    Hotel = x.HotelNavn,
                    Rolle = x.Rolle,
                    Status = x.Status
                });
            }
            return Ok(brugers);
        }
        
        
        /// <summary>
        /// Finder en bruger efter email og sender resetkode til denne mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns>En bruger </returns>
        [HttpGet]
        [Route("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email er blank");
            }
            
            var user = await _userRepository.GetUserByEmail(email);
            
            if (user == null)
            {
                return NotFound();
            }
            
            //Send reset kode
            await SendResetCodeEmail(email, GenerateResetCode(email));
            
            return Ok(user);
        }

        
        /// <summary>
        /// Finder en bruger
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retunererer en bruger</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id er forkert");
            }
            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound("Kunne ikke finde brugeren");
            }
            return Ok(user);
        }

        /// <summary>
        /// Metode til at opret en bruger
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retuner en bruger</returns>
        [HttpPost]
        public async Task<IActionResult> PostUser(BrugerCreateDTO user)
        {
            //Validering
            if (string.IsNullOrWhiteSpace(user.FirstName))
                return Conflict("Venligst indtast et fornavn");

            if (string.IsNullOrWhiteSpace(user.LastName))
                return Conflict("Venligst indtast et efternavn");

            if (string.IsNullOrWhiteSpace(user.Email))
                return Conflict("Venligst indtast en e-mail");

            if (!user.Mobile.HasValue)
                return Conflict("Venligst indtast et mobilnummer");
            
            var mobileStr = user.Mobile.Value.ToString();
            if (mobileStr.Length != 8)
                return Conflict("Mobilnummer skal være 8 cifre");

            if (string.IsNullOrWhiteSpace(user.Rolle))
                return Conflict("Venligst vælg en rolle");

            if (string.IsNullOrWhiteSpace(user.Køn))
                return Conflict("Venligst angiv et køn");

            if (!user.Email.Contains("@"))
            {
                return Conflict("Dette er ikke en valid email");
            }
            
            // Tjekker unik email
            var result = await _userRepository.CheckUnique(user.Email);
            if (!result)
                return Conflict("Du har en bruger");

            if (user.Rolle == "Elev")
            {
                if (user.StartDate == null)
                    return Conflict("Venligst indsæt en startdato");

                if (user.EndDate == null)
                    return Conflict("Venligst angiv en slutdato");
                
                if (user.StartDate > user.EndDate)
                    return Conflict("Mismatch i start og slutdato");
                
                if (string.IsNullOrWhiteSpace(user.Year))
                    return Conflict("Venligst angiv en årgang");

                if (string.IsNullOrWhiteSpace(user.Skole))
                    return Conflict("Venligst angiv en skole");

                if (string.IsNullOrWhiteSpace(user.Uddannelse))
                    return Conflict("Venligst angiv en uddannelse");
            }
            
            
            var nyBruger = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                Email = user.Email,
                Password = GeneratePassword(),
                Rolle = user.Rolle,
           
            };

            if (user.Rolle == "HR")
            {
                nyBruger.HotelId = null;
            }

            Hotel hotel = null;

            if (user.Rolle != "HR")
            {
                if (user.HotelId <= 0)
                {
                    return BadRequest("Venligst indsæt et hotel");
                }
                
                hotel = await _hotelRepository.GetHotelById(user.HotelId);

                if (hotel == null)
                {
                    return NotFound("Kunne ikke finde hotellet");
                }
                
                nyBruger.HotelId = user.HotelId;
                nyBruger.HotelNavn = hotel?.HotelNavn;
            }

            if (user.Rolle == "Elev")
            {
                nyBruger.StartDate = user.StartDate;
                nyBruger.EndDate = user.EndDate;
                nyBruger.Year = user.Year;
                nyBruger.Skole = user.Skole;
                nyBruger.Uddannelse = user.Uddannelse;
            }

            if (user.Rolle == "Køkkenchef")
            {
                if (hotel != null && (hotel.KøkkenChefId != null || !string.IsNullOrEmpty(hotel.KøkkenChefNavn)))
                {
                    return Conflict("Dette hotel har allerede en køkkenchef");
                }
            }
    
            var newUser = await _userRepository.SaveBruger(nyBruger);

            if (newUser == null)
            {
                return BadRequest("Kunne ikke gemme den nye bruger, prøv igen");
            }
            
    
            if (user.Rolle == "Køkkenchef" && hotel != null)
            {
                hotel.KøkkenChefId = newUser.Id;
                hotel.KøkkenChefNavn = newUser.FirstName + " " + newUser.LastName;
                var updateResult = await _hotelRepository.UpdateHotelChef(hotel);

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound("Kunne ikke opdatere hotelchefen");
                }
            }

            //Sender mail med logindetaljer
            var mailSent = await SendLoginDetails(nyBruger);

            if (!mailSent)
            {
                return Conflict("Dette er ikke en valid mail");
            }
            
            return Ok(newUser);
        }

        /// <summary>
        /// Sletter en bruger, hvis brugeren er køkkenchef, så fjerner vi også fra hotellet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rolle"></param>
        /// <returns>Stauts 200</returns>
        [HttpDelete]
        [Route("{userId}/{rolle}")]
        public async Task<IActionResult> DeleteUser(int userId, string rolle)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(rolle))
            {
                return BadRequest("Forkert userId eller rolle er blank");
            }
            
            if (rolle == "Køkkenchef")
            {
                var updateResult = await _hotelRepository.RemoveChefFromHotel(userId);

                if (updateResult.MatchedCount == 0)
                {
                    return NotFound("Kunne ikke finde køkkenchefen");
                }
                
            }
            var deleteResult = await _userRepository.DeleteUser(userId);

            if (deleteResult.DeletedCount == 0)
            {
                return NotFound("Kunne ikke slette brugeren");
            }
            
            return Ok();
        }

        /// <summary>
        /// Deaktiver en bruger der har status aktiv
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Stauts 200</returns>
        [HttpPut]
        [Route("deactivate/{userId}/{rolle}")]
        public async Task<IActionResult> DeactivateUser(int userId, string rolle)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(rolle))
            {
                return BadRequest("Forkert userId eller rolle er blank");
            }
            
            if (rolle == "Køkkenchef")
            {
                var hotelUpdate = await _hotelRepository.RemoveChefFromHotel(userId);

                if (hotelUpdate.MatchedCount == 0)
                {
                    return NotFound("Kunne ikke fjerne køkkenchefen fra hotellet");
                }
                
            }
            
            var deactiveResult = await _userRepository.DeactivateUser(userId);

            if (deactiveResult.MatchedCount == 0)
            {
                return NotFound("Kunne ikke deaktivere brugeren");
            }
            
            return Ok();
        }

        /// <summary>
        /// Aktiver en bruger der er deaktiveret
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Stauts 200</returns>
        [HttpPut]
        [Route("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Forkert userId");
            }
            
            var activateResult = await _userRepository.ActivateUser(userId);

            if (activateResult.MatchedCount == 0)
            {
                return NotFound("Kunne ikke finde brugeren");
            }
            
            
            return Ok();

        }

        /// <summary>
        /// Ændrer rolle på en bruger
        /// </summary>
        /// <returns>Stauts 200</returns>
        [HttpPut]
        [Route("updaterolle/{userId}/{newRolle}")]
        public async Task<IActionResult> UpdateRole(string newRolle, int userId)
        {
            if (string.IsNullOrWhiteSpace(newRolle) || userId <= 0)
            {
                return BadRequest("Forkert userId eller rolle er blank");
            }
            
            var updateResult = await _userRepository.UpdateRolle(newRolle, userId);

            if (updateResult.MatchedCount == 0)
            {
                return NotFound("Kunne ikke finde brugeren");
            }
            
            
            return Ok();
        }
        
        /// <summary>
        /// Opdater password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="updatedPassword"></param>
        /// <returns>UpdateResult</returns>
        [HttpPut]
        [Route("updatepassword/{email}")]
        public async Task<IActionResult> UpdatePassword(string email, [FromBody] string updatedPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Emailen er blank");
            }
            
            if (string.IsNullOrWhiteSpace(updatedPassword))
            {
                return BadRequest("Opdateret password er blank");
            }

            if (updatedPassword.Length < 8)
            {
                return BadRequest("Password skal være 8 cifre eller mere");
            }
            
            var result = await _userRepository.UpdatePassword(email, updatedPassword);

            if (result.MatchedCount == 0)
            {
                return NotFound("Kunne ikke finde brugeren");
            }
            
            return Ok(result);

        }

        /// <summary>
        /// Metode til at tjekke, at vores verificeringskoder er rigtige
        /// </summary>
        /// <returns>true eller false</returns>
        [HttpGet]
        [Route("{email}/{kode}")]
        public async Task<IActionResult> CheckVerificationCoed(string email, string kode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(kode))
            {
                return BadRequest("Emailen eller verificeringskoden er blank");
            }
            
            if (verificeringsKoder.TryGetValue(email, out var output))
            {
                if (output.Kode == kode && output.Expiry > DateTime.Now)
                {
                    return Ok(true);
                }
            }

            return BadRequest(false);
        }

        /// <summary>
        /// Henter alle elever og konverter til DTO
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allstudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            List<KursusDeltagerListeDTO> students = new();
            var users = await _userRepository.GetAllStudents();
            

            foreach (var user in users)
            {
                students.Add(new KursusDeltagerListeDTO
                {
                    Id = user.Id,
                    Hotel = user.HotelNavn,
                    Navn = user.FirstName + "  " + user.LastName
                });
            }

            return Ok(students);
        }
        

        /// <summary>
        /// Henter elevoversigten for HR
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("oversigt")]
        public async Task<IActionResult> GetElevOversigt()
        {
            var users = await _userRepository.GetAllStudents();
            
            var elevOversigt = new List<ElevOversigtDTO>();

            foreach (var elev in users.Where(x => x.Rolle == "Elev"))
            {
                elevOversigt.Add(new ElevOversigtDTO
                {
                    Id = elev.Id,
                    Name = elev.FirstName + " " + elev.LastName,
                    HotelId = elev.HotelId,
                    HotelNavn = elev.HotelNavn,
                    Roller = elev.Rolle,
                    Year = elev.Year,
                    Skole = elev.Skole,
                    Uddannelse = elev.Uddannelse,
                    StartDate = elev.StartDate,
                    EndDate = elev.EndDate,
                    TotalGoals = elev.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count) ?? 0,
                    CompletedGoals =
                        elev.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count(g => g.Status == "Completed")) ?? 0,
                });
            }

            return Ok(elevOversigt);
        }
        
        /// <summary>
        /// Finder elevoversigten pr hotel
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("oversigt/{hotelId}")]
        public async Task<IActionResult> GetElevOversigtByHotel(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest("Forkert hotelId");
            }
            
            var elever = await _userRepository.GetAllStudentsByHotelId(hotelId);
            
            List<ElevOversigtDTO> elevOversigt = new();

            foreach (var elev in elever)
            {
                elevOversigt.Add(new ElevOversigtDTO
                {
                    Id = elev.Id,
                    Name = elev.FirstName + " " + elev.LastName,
                    HotelId = elev.HotelId,
                    HotelNavn = elev.HotelNavn,
                    Roller = elev.Rolle,
                    Year = elev.Year,
                    Skole = elev.Skole,
                    Uddannelse = elev.Uddannelse,
                    StartDate = elev.StartDate,
                    EndDate = elev.EndDate,
                    TotalGoals = elev.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count) ?? 0,
                    CompletedGoals =
                        elev.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count(g => g.Status == "Completed")) ?? 0,
                });
            }
            
            return Ok(elevOversigt);
        }

        
        /// <summary>
        /// Sender en mail med oplysninger 
        /// </summary>
        /// <param name="studentIds"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendemail/{email}")]
        public async Task<IActionResult> SendEmail([FromBody] HashSet<int> studentIds, string email)
        {
            if (!studentIds.Any() || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Emailen er blank eller ingen studentids");
            }
            
            var students = await _userRepository.GetAllStudents();

            if (!students.Any())
            {
                return NotFound("Kunne ikke finde nogen elever");
            }
            
            var filter = students.Where(x => studentIds.Contains(x.Id)).ToList();

            var emailStatus = await GenerateExcelFileAndSendMail(filter, email);
            
            return Ok(emailStatus);

        }

        /// <summary>
        /// Opdater en users hotel
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotelId"></param>
        /// <param name="hotelNavn"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatehotel/{userId}/{hotelId}")]
        public async Task<IActionResult> UpdateUser(int userId, int hotelId, [FromBody] string hotelNavn)
        {
            var result = await _userRepository.UpdateHotel(userId, hotelId, hotelNavn);

            if (result.MatchedCount == 0)
            {
                return NotFound("Kunne ikke finde hotel");
            }
            return Ok(result);
        }

        /// <summary>
        /// Finder alle aktive users
        /// </summary>
        /// <returns>BrugerLoginDTO</returns>
        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            var activeUsers = await _userRepository.GetAllActiveUsers();
            
            List<BrugerLoginDTO> users = new();

            foreach (var user in activeUsers)
            {
                users.Add(new BrugerLoginDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    Email = user.Email,
                    Password = user.Password,
                    HotelId = user.HotelId,
                    Rolle = user.Rolle
                });
                
            }
            return Ok(users);
        }
        
        [NonAction]
        //Hjælpefunktion til at reset email
        public async Task SendResetCodeEmail(string email, string verificeringsKode)
        {

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            //Anvender SendGrid
            var client = new SendGridClient(apiKey);

            //Fra & Til
            var from = new EmailAddress("jonasdupontheidemann@gmail.com", "HR");
            var to = new EmailAddress(email);

            //Indhold
            var subject = "Nulstilling af Comwell adgangskode";
            var plainTextContent =
                $"Opret din nye adgangskode\t\n\t\t\n\t" +
                $"Vi skriver til dig fordi du har oplyst, at du har glemt din adgangskode til din Comwell profil." +
                $"\n\nDu skal bruge følgende midlertidige kode til at oprette din nye adgangskode:\t\n " +
                $"{verificeringsKode}" +
                $"\t\nHar du ikke anmodet om en ny adgangskode til Comwell login, kan du se bort fra denne mail.\t";

            var htmlContent = $"{verificeringsKode}";

            //Generer email og sender
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        [NonAction]
        //Checker vores verificeringskode... i server memory
        public async Task<bool> CheckVerficiationCode(string email, string kode)
        {
            if (verificeringsKoder.TryGetValue(email, out var output))
            {
                if (output.Kode == kode && output.Expiry > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        //Generer verificeringskode
        [NonAction]
        public string GenerateResetCode(string email)
        {
            Random ran = new Random();
            string verificeringsKode = String.Empty;

            string bogstaver = "abcdefghijklmnopqrstuvwxyz0123456789";
            int size = 8;

            for (int i = 0; i < size; i++)
            {
                //Tager et random index
                int x = ran.Next(bogstaver.Length);
                verificeringsKode = verificeringsKode + bogstaver[x];
            }
                
            verificeringsKoder[email] = (verificeringsKode, DateTime.Now.AddMinutes(10));
            
            return verificeringsKode;
        }
        
        //Opretter password
        [NonAction]
        public string GeneratePassword()
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var password = "";

            for (int i = 0; i < 8; i++)
            {
                var index = random.Next(chars.Length);
                password += chars[index];
            }

            return password;
        }
        
        
        [NonAction]
        public async Task<bool> SendLoginDetails(User newUser)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            //Anvender SendGrid
            var client = new SendGridClient(apiKey);

            //Fra & Til
            var from = new EmailAddress("jonasdupontheidemann@gmail.com", "HR");
            var to = new EmailAddress(newUser.Email);

            //Indhold
            var subject = "Velkommen til Elevportalen";
            var plainTextContent =
                $"Opret din nye adgangskode\t\n\t\t\n\t" +
                $"Vi skriver til dig fordi du har oplyst, at du har glemt din adgangskode til din Comwell profil." +
                $"\n\nDu skal bruge følgende midlertidige kode til at oprette din nye adgangskode:\t\n " +
                $"\t\nHar du ikke anmodet om en ny adgangskode til Comwell login, kan du se bort fra denne mail.\t";

            var htmlContent = $"Hejsa, du er hermed  oprettet i vores system\t UserName: {newUser.Email}\n\n {newUser.Password}";

            //Generer email og sender
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        
        //Generer excel fil
        [NonAction]
        public async Task<bool> GenerateExcelFileAndSendMail(List<User> users, string email)
        {
            byte[] fileBytes;
            
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dashboard");
                
                //Overskrifter
                worksheet.Cell("A1").Value = "Navn";
                worksheet.Cell("B1").Value = "Hotel";
                worksheet.Cell("C1").Value = "Status"; 
                worksheet.Cell("D1").Value = "År";
                worksheet.Cell("E1").Value = "Skole";
                worksheet.Cell("F1").Value = "Uddannelse";
                worksheet.Cell("G1").Value = "Start";
                worksheet.Cell("H1").Value = "Slut";

                //Data 
                int row = 2;
                foreach (var user in users)
                {
                    //Alle mål og gennemførte
                    var TotalGoals = user.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count) ?? 0;
                    var CompletedGoals =
                        user.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count(g => g.Status == "Completed")) ?? 0;
                    
                    worksheet.Cell(row, 1 ).Value = user.FirstName + " " + user.LastName;
                    worksheet.Cell(row, 2 ).Value = user.HotelNavn;
                    worksheet.Cell(row, 3 ).Value = $"{CompletedGoals} / {TotalGoals}";
                    worksheet.Cell(row, 4 ).Value = user.Year;
                    worksheet.Cell(row, 5 ).Value = user.Skole;
                    worksheet.Cell(row, 6 ).Value = user.Uddannelse;
                    worksheet.Cell(row, 7 ).Value = user.StartDate.ToString();
                    worksheet.Cell(row, 8 ).Value = user.EndDate.ToString();
                    row++;
                }
                
                //Juster kolonner, så de fitter
                worksheet.Columns().AdjustToContents();


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    fileBytes = stream.ToArray();
                }
                
            }
            
            string base64 = Convert.ToBase64String(fileBytes);
            
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            
            var from = new EmailAddress("jonasdupontheidemann@gmail.com", "Jonas Heidemann");
            var subject = "Alle produkter";
            var to = new EmailAddress(email, "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            
            msg.AddAttachment("data.xlsx", base64, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            
            var sendEmail = await client.SendEmailAsync(msg);

            return true;
        }
        


        
    }

}