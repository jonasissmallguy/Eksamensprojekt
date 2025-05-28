using System.Net;
using System.Net.Http.Json;
using Core;
using Blazored.LocalStorage;
using SendGrid.Helpers.Errors.Model;

namespace Client
{

    public class BrugerServiceServer : IBruger
    {
        private HttpClient _client = new();
        
        public BrugerServiceServer(HttpClient client)
        {
            _client = client;
        }

        public async Task<User> GetBrugerById(int userId)
        {
            var result =  await _client.GetAsync($"users/{userId}");

            if (!result.IsSuccessStatusCode)
            {
                var message = await result.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
            
            var user = await result.Content.ReadFromJsonAsync<User>();
            return user;
        }
        
        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        { 
            HttpResponseMessage response = await _client.PostAsJsonAsync($"users", nyBruger);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
    
            User createdUser = await response.Content.ReadFromJsonAsync<User>();
    
            return createdUser;
        }
        
        public async Task<List<ElevOversigtDTO>> GetElevOversigt()
        {

            try
            {
                var result = await _client.GetFromJsonAsync<List<ElevOversigtDTO>>($"users/oversigt");
                return result ?? new List<ElevOversigtDTO>();
            }
            
            catch (HttpRequestException)
            {
                return new List<ElevOversigtDTO>();
            }
            
        }

        public async Task<List<ElevOversigtDTO>> GetElevOversigtByHotelId(int? hotelId)
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<ElevOversigtDTO>>($"users/oversigt/{hotelId}");
                
                return result ?? new List<ElevOversigtDTO>();
                
            }
            catch (HttpRequestException)
            {
                return new List<ElevOversigtDTO>();
            }
        }

        public async Task<List<BrugerLoginDTO>> GetAllActiveUsers()
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<BrugerLoginDTO>>($"users/active");
                return result ?? new List<BrugerLoginDTO>();
                
            }
            catch (HttpRequestException)
            {
                return new List<BrugerLoginDTO>();
            }
        }

        public async Task<List<BrugerAdministrationDTO>> GetAllUsersWithOutCurrent(int userId)
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<BrugerAdministrationDTO>>($"users/withoutmyself/{userId}");
                return result ?? new List<BrugerAdministrationDTO>();
            }
            catch (HttpRequestException)
            {
                return new List<BrugerAdministrationDTO>();
            }
        }
        

        public async Task<bool> DeleteUser(int userId, string rolle)
        {
            var result = await _client.DeleteAsync($"users/{userId}/{rolle}");

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public async Task ChangeRolle(string newRolle, int userId)
        {
            await _client.PutAsJsonAsync($"users/updaterolle/{userId}/{newRolle}", new{});
        }

        public async Task DeActivateUser(int userId, string rolle)
        { 
            await _client.PutAsJsonAsync($"users/deactivate/{userId}/{rolle}", userId);
        }

        public async Task ActivateUser(int userId)
        {
            await _client.PutAsJsonAsync($"users/activate/{userId}", userId);
        }

        public async Task<bool> UpdateHotel(int hotelId, string hotelName, int userId)
        {
            var result = await _client.PutAsJsonAsync($"users/updatehotel/{userId}/{hotelId}", hotelName);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            
            return true;
        }

        

        public async Task<List<KursusDeltagerListeDTO>> GetAllStudents()
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<KursusDeltagerListeDTO>>($"users/allstudents");
                
                return result ?? new List<KursusDeltagerListeDTO>();
                
            }
            catch (HttpRequestException)
            {
                return new List<KursusDeltagerListeDTO>();
            }
        }

        public async Task<List<KursusDeltagerListeDTO>> GetAllStudentsMissingCourse(string courseCode)
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<KursusDeltagerListeDTO>>($"goals/allstudents/{courseCode}");
                
                return result ?? new List<KursusDeltagerListeDTO>();
                
            }

            catch (HttpRequestException)
            {
                return new List<KursusDeltagerListeDTO>();
            }
            
        }

        public async Task<bool> SendEmail(HashSet<int> studentIds, string email)
        {
            var response = await _client.PostAsJsonAsync($"users/sendemail/{email}", studentIds);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }

}