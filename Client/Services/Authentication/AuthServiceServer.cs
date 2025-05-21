using System.Net;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Core;

namespace Client
{

    public class AuthServiceServer : IAuth
    {
        
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client;
        private ILocalStorageService _localStorage;
        private IBruger _bruger;
        private readonly string _serverUrl;

        public AuthServiceServer(HttpClient client, ILocalStorageService localStorage, IBruger bruger)
        {
            _client = client;
            _localStorage = localStorage;
            _bruger = bruger;
        }

        public async Task<BrugerLoginDTO?> GetBruger()
        {
            BrugerLoginDTO? loggedInBruger = await _localStorage.GetItemAsync<BrugerLoginDTO>("bruger");

            if (loggedInBruger == null)
            {
                return null;
            }

            return new BrugerLoginDTO
            {
                Id = loggedInBruger.Id,
                Email = loggedInBruger.Email,
                Password = loggedInBruger.Password,
                Rolle = loggedInBruger.Rolle,
                FirstName = loggedInBruger.FirstName,
                HotelId = loggedInBruger.HotelId,
            };
        }
        
        //Hjælpefunktion til at validere om 
        public async Task<BrugerLoginDTO?> Validate(string username, string password)
        {
            var _allUsers = await _bruger.GetAllUsers();
            
            foreach (User bruger in _allUsers)
            {
                if (bruger.Email.Equals(username) && bruger.Password.Equals(password))
                {
                    return new BrugerLoginDTO
                    {
                        Id = bruger.Id,
                        Email = username,
                        Password = password,
                        Rolle = bruger.Rolle,
                        FirstName  = bruger.FirstName,
                        HotelId = bruger.HotelId
                    };
                }
            }
            return null;
        }

        public async Task<BrugerLoginDTO> Login(string username, string password)
        {
            BrugerLoginDTO? bruger = await Validate(username, password);

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

        public async Task GetUserByEmail(string email)
        {
           var user = await _client.GetFromJsonAsync<User>($"{serverUrl}/users/{email}");
    
           if (user != null)
           {
               await _localStorage.SetItemAsync("resetEmail", user.Email); 
               
           }
        }

        public async Task<bool> CheckVerficiationCode(string email, string kode)
        {
            var result = await _client.GetFromJsonAsync<bool>($"{serverUrl}/users/{email}/{kode}");

            if (!result)
            {
                return false;
            }

            return true;
        }

        public async Task<string> GetLocalStorageResetEmail()
        {
            var userToResetEmail = await _localStorage.GetItemAsync<string>("resetEmail");
            return userToResetEmail;
        }

        public async Task DeleteLocalStorageResetEmail()
        {
            await _localStorage.RemoveItemAsync("resetEmail");
        }

        public async Task<bool> UpdatePassword(string updatedPassword, string confirmedPassword)
        {
            BrugerLoginDTO currentUser = await GetBruger();
            
            //Bruger er allerede logget ind
            if (currentUser != null)
            {
                var result = await _client.PutAsJsonAsync($"{serverUrl}/users/updatepassword/{currentUser.Email}", updatedPassword);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            //Bruger resetter men er ikke logget ind
            else
            {
                var email = await GetLocalStorageResetEmail();
                var result = await _client.PutAsJsonAsync($"{serverUrl}/users/updatepassword/{email}", updatedPassword);
                
                //var user = await _client.GetFromJsonAsync<User>($"{serverUrl}/users/{email}");

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
               
            }
            return false;
        }
    }

}