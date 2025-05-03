using Core;
using Blazored.LocalStorage;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DotNetEnv;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Client
{

    public class LoginServiceMock : ILogin
    {

        private List<Bruger> _allUsers = new List<Bruger>
        {
            new Bruger
            {
                Id = 1,
                Email = "admin@admin.com",
                Password = "123456",
                Rolle = "HR"
            },
            new Bruger
            {
                Id = 2,
            Email = "elev1@admin.com",
            Password = "123456",
            Rolle = "Elev"
            },
            new Bruger
            {
                Id = 3,
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Mentor"
            },
            new Bruger
            {
                Id = 4,
                Email = "tjoernevej53@gmail.com",
                Password = "123456",
                Rolle = "Mentor"
            }
            
        };
        
        //Verificeringskoder
        private static Dictionary<string, (string Kode, DateTime Expiry)> verificeringsKoder = new();

        
        private  ILocalStorageService _localStorage;

        public LoginServiceMock(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        
        public async Task<BrugerLoginDTO?> GetBruger()
        {
            Bruger? loggedInBruger = await _localStorage.GetItemAsync<Bruger>("bruger");

            if (loggedInBruger == null)
            {
                return null;
            }

            return new BrugerLoginDTO
            {
                Id = loggedInBruger.Id,
                Email = loggedInBruger.Email,
                Password = loggedInBruger.Password,
                Rolle = loggedInBruger.Rolle
            };
        }
        
        //Hjælpefunktion til at validere om 
        public virtual async Task<BrugerLoginDTO?> Validate(string username, string password)
        {
            foreach (Bruger bruger in _allUsers)
            {
                if (bruger.Email.Equals(username) && bruger.Password.Equals(password))
                {
                    return new BrugerLoginDTO
                    {
                        Id = bruger.Id,
                        Email = username,
                        Password = password,
                        Rolle = bruger.Rolle
                    };
                }
            }
            return null;
        }

        public async Task<BrugerLoginDTO> Login(string username, string password)
        {
            
            BrugerLoginDTO? bruger  = await Validate(username, password);

            if (bruger != null)
            {
                bruger.Password = "validated";
                await _localStorage.SetItemAsync("bruger", bruger);
                return bruger;
            }
            return null;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("bruger");
        }
        
        //Hjælpe funktion til at reset password
        public async Task SendResetCodeEmail(string email, string verificeringsKode)
        {
            //Loader .env
            Env.Load();
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
            
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            
            //Generer email
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task GetUserByEmail(string email)
        {
            var user = _allUsers.FirstOrDefault(u => u.Email.Equals(email));

            if (user != null)
            {
                //Generer kode på 6 cifre til at resette password
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
                
                //Kalder hjælpefunktion til at sende email - kan ikke virker i mock
                //await SendResetCodeEmail(email, verificeringsKode);
                
                Console.WriteLine(verificeringsKode);
            }
        }

        public async Task<bool> CheckVerficiationCode(string kode)
        {
            if (verificeringsKoder.TryGetValue(kode, out var input))
            {
                if (input.Kode == kode && input.Expiry > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }
    }

}