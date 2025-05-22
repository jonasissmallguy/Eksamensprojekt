using Core;
using Microsoft.AspNetCore.Mvc;
using DotNetEnv;
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

            if (allUsers == null)
            {
                return NotFound();
            }

            return Ok(brugerLogins);
        }

        /// <summary>
        /// Henter alle bruger uden mig selv
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("withoutmyself/{id:int}")]
        public async Task<IActionResult> GetAllUsersWithOutMyself(int id)
        {
            List<BrugerAdministrationDTO> brugers = new();

            var allUsers = await _userRepository.GetAllUsersWithOutMyself(id);

            if (allUsers == null)
            {
                return NotFound();
            }

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

        [NonAction]
        //Generer verificeringskode
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
        
        [HttpGet]
        [Route("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            

            if (user == null)
            {
                return NotFound();
            }
            
            //Send reset kode
            await SendResetCodeEmail(email, GenerateResetCode(email));
            
            return Ok(user);
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            
            return Ok(user);
        }

        [NonAction]
        //Opretter password
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
        

        /// <summary>
        /// Metode til at opret en bruger
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

            if (string.IsNullOrWhiteSpace(user.Rolle))
                return Conflict("Venligst vælg en rolle");

            if (string.IsNullOrWhiteSpace(user.Køn))
                return Conflict("Venligst angiv et køn");

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

            Hotel hotel = null;
            hotel = await _hotelRepository.GetHotelById(user.HotelId);

            if (hotel == null)
            {
                return BadRequest();
            }

            var nyBruger = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                Email = user.Email,
                Password = GeneratePassword(),
                Rolle = user.Rolle,
                HotelId = user.HotelId,
                HotelNavn = hotel?.HotelNavn
            };

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
    
            if (user.Rolle == "Køkkenchef" && hotel != null)
            {
                hotel.KøkkenChefId = newUser.Id;
                hotel.KøkkenChefNavn = newUser.FirstName + " " + newUser.LastName;
                await _hotelRepository.UpdateHotelChef(hotel);
            }

            /*var mailSent = await SendLoginDetails(nyBruger);

            if (!mailSent)
            {
                return BadRequest();
            }
            */
    
            return Ok(newUser);
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
        

        /// <summary>
        /// Sletter en bruger
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{userId}/{rolle}")]
        public async Task<IActionResult> DeleteUser(int userId, string rolle)
        {
            if (rolle == "Køkkenchef")
            {
                await _hotelRepository.RemoveManagerFromHotel(userId);
            }
            await _userRepository.DeleteUser(userId);
            return Ok();
        }

        /// <summary>
        /// Deaktiver en bruger der har status aktiv
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deactivate/{userId}/{rolle}")]
        public async Task<IActionResult> DeactivateUser(int userId, string rolle)
        {
            if (rolle == "Køkkenchef")
            {
                await _hotelRepository.RemoveManagerFromHotel(userId);
            }
            
            await _userRepository.DeactivateUser(userId);

            return Ok();
        }

        /// <summary>
        /// Aktiver en bruger der er deaktiveret
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            await _userRepository.ActivateUser(userId);

            return Ok();

        }

        /// <summary>
        /// Ændrer rolle på en bruger
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updaterolle/{userId}/{newRolle}")]
        public async Task<IActionResult> UpdateRole(string newRolle, int userId)
        {
            await _userRepository.UpdateRolle(newRolle, userId);

            return Ok();
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateUser([FromBody] User updateBruger)
        {
            var user = await _userRepository.UpdateUser(updateBruger);

            return Ok(user);
        }


        [HttpPut]
        [Route("updatepassword/{email}")]
        public async Task<IActionResult> UpdatePassword(string email, [FromBody] string updatedPassword)
        {
            var result = await _userRepository.UpadtePassword(email, updatedPassword);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);

        }

        /// <summary>
        /// Metode til at tjekke, at vores verificeringskoder er rigtige
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{email}/{kode}")]
        public async Task<bool> CheckVerificationCoed(string email, string kode)
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

        //BRUGES KUN TIL TEST MENS VI VENTER!!
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
            
            Env.Load();
            
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            
            //fra og til skal ændres dynamisk... brugt til test.
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
        
        [HttpPost]
        [Route("sendemail/{email}")]
        public async Task<IActionResult> SendEmail([FromBody] HashSet<int> studentIds, string email)
        {
            //Skal laves specifik til hvem der ser??
            var students = await _userRepository.GetAllStudents();
            
            var filter = students.Where(x => studentIds.Contains(x.Id)).ToList();

            var emailStatus = await GenerateExcelFileAndSendMail(filter, email);
            
            return Ok(emailStatus);

        }

        [HttpPut]
        [Route("updatehotel/{userId}/{hotelId}")]
        public async Task<IActionResult> UpdateUser(int userId, int hotelId, [FromBody] string hotelNavn)
        {
            var result = await _userRepository.UpdateHotel(userId, hotelId, hotelNavn);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            var activeUsers = await _userRepository.GetAllActiveUsers();

            if (activeUsers == null)
            {
                return NotFound();
            }

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
        
        
    }

}