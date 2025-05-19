using Core;
using Microsoft.AspNetCore.Mvc;
using DotNetEnv;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            var allUsers =  await _userRepository.GetAllUsers();

            if (allUsers == null)
            {
                return NotFound();
            }
            return Ok(allUsers);
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
            var allUsers = await _userRepository.GetAllUsersWithOutMyself(id);

            if (allUsers == null)
            {
                return NotFound();
            }
            return Ok(allUsers);
        }
  
        //Hjælpefunktion til at reset email
        public async Task SendResetCodeEmail(string email, string verificeringsKode)
        {
            //Loader .env
            Env.Load();
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            
            //Anvender SendGrid
            var client = new SendGridClient(apiKey);

            //Fra & Til
            var from = new EmailAddress(email, "HR");
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
            await SendResetCodeEmail("jonasdupontheidemann@gmail.com", GenerateResetCode(email));
            
            return Ok(user);
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            
            return Ok(user);
        }

        
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
                HotelNavn = hotel?.HotelNavn,
                //mangler hotel
                Year = user.Year,
                StartDate = user.StartDate,
                EndDate = user.EndDate,
                Skole = user.Skole,
                Uddannelse = user.Uddannelse
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
    
            return Ok(newUser);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);

            return Ok();
        }

        /// <summary>
        /// Deaktiver en bruger der har status aktiv
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
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
        
    }

}