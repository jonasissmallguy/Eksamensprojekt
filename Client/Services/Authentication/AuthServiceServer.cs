using System.Net;
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
                FirstName = loggedInBruger.FirstName
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
                        FirstName  = bruger.FirstName
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

        public Task GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckVerficiationCode(string email, string kode)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetLocalStorageResetEmail()
        {
            throw new NotImplementedException();
        }

        public Task DeleteLocalStorageResetEmail()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePassword(string updatedPassword, string confirmedPassword)
        {
            throw new NotImplementedException();
        }
    }

}