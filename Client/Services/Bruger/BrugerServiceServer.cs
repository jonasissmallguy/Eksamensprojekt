using System.Net;
using System.Net.Http.Json;
using Core;
using Blazored.LocalStorage;
using SendGrid.Helpers.Errors.Model;

namespace Client
{

    public class BrugerServiceServer : IBruger
    {
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client = new();
        
        private List<User> _allUsers = new();

        public BrugerServiceServer(HttpClient client)
        {
            _client = client;
        }

        public async Task<User> GetBrugerById(int userId)
        {
            return await _client.GetFromJsonAsync<User>($"{serverUrl}/users/{userId}");
        }

        public async Task<bool> OpdaterBruger(int userId, User updateBruger)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/users/update", updateBruger);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update failed: {response.StatusCode} - {errorContent}");
                return false;
            }

            return true;
        }


        
        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        { 
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{serverUrl}/users", nyBruger);

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
            var allUsers = await _client.GetFromJsonAsync<List<User>>($"{serverUrl}/users");
            
            var elevOversigt = new List<ElevOversigtDTO>();
            
            var elever = allUsers.Where(x => x.Rolle == "Elev").ToList();

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
                    TotalGoals = elev.ElevPlan?.Forløbs?.Sum(f=> f.Goals?.Count) ?? 0,
                    CompletedGoals = elev.ElevPlan?.Forløbs?.Sum(f => f.Goals?.Count(g=> g.Status == "Completed")) ?? 0,
                    
                });
            }

            return await Task.FromResult(elevOversigt);
        }

        public async Task<List<BrugerLoginDTO>> GetAllUsers()
        {
            return await _client.GetFromJsonAsync<List<BrugerLoginDTO>>($"{serverUrl}/users");
        }

        public async Task<List<BrugerLoginDTO>> GetAllActiveUsers()
        {
            return await _client.GetFromJsonAsync<List<BrugerLoginDTO>>($"{serverUrl}/users/active");
        }

        public async Task<List<BrugerAdministrationDTO>> GetAllUsersWithOutCurrent(int userId)
        {
            return await _client.GetFromJsonAsync<List<BrugerAdministrationDTO>>($"{serverUrl}/users/withoutmyself/{userId}");
        }

        public async Task<List<User>> GetAllUsersByStudentId(List<int> studentIds)
        {
            var users = new List<User>();

            foreach (var id in studentIds)
            {
                var user = _allUsers.FirstOrDefault(x => x.Id == id);

                if (user != null)
                {
                    users.Add(user);
                }
            }
            return users;
        }

        public async Task<bool> DeleteUser(int userId, string rolle)
        {
            var result = await _client.DeleteAsync($"{serverUrl}/users/{userId}/{rolle}");

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public async Task ChangeRolle(string newRolle, int userId)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/users/updaterolle/{userId}/{newRolle}", new{});
        }

        public async Task DeActivateUser(int userId, string rolle)
        { 
            await _client.PutAsJsonAsync($"{serverUrl}/users/deactivate/{userId}/{rolle}", userId);
        }

        public async Task ActivateUser(int userId)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/users/activate/{userId}", userId);
        }

        public async Task<bool> UpdateHotel(int hotelId, string hotelName, int userId)
        {
            var result = await _client.PutAsJsonAsync($"{serverUrl}/users/updatehotel/{userId}/{hotelId}", hotelName);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            
            return true;
        }

        public Task SaveStudentPlan(int studentId, Plan plan)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(int currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<KursusDeltagerListeDTO>> GetAllStudents()
        {
            return await _client.GetFromJsonAsync<List<KursusDeltagerListeDTO>>($"{serverUrl}/users/allstudents");
        }
        
        //Skal også tage brugerens email som parameter, altså hvem er logget ind og hvem skal vi sendetil!

        public async Task<bool> SendEmail(HashSet<int> studentIds)
        {
            var response = await _client.PostAsJsonAsync($"{serverUrl}/users/sendemail", studentIds);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public Task<List<User>> GetStudentsForløb(int leaderId)
        {
            throw new NotImplementedException();
        }
    }

}