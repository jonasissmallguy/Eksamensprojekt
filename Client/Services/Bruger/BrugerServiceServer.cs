using System.Net;
using System.Net.Http.Json;
using Core;
using SendGrid.Helpers.Errors.Model;

namespace Client
{

    public class BrugerServiceServer : IBruger
    {
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client = new();

        public BrugerServiceServer(HttpClient client)
        {
            _client = client;
        }

        public async Task<User> GetBrugerById(int userId)
        {
            return await _client.GetFromJsonAsync<User>($"{serverUrl}/users/{userId}");
        }

        public Task<bool> OpdaterBruger(int userId, User updateBruger)
        {
            throw new NotImplementedException();
        }

        public Task<List<ManagerDTO>> GetAllManagers()
        {
            throw new NotImplementedException();
        }
        
        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        { 
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{serverUrl}/users", nyBruger);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
    
            User createdUser = await response.Content.ReadFromJsonAsync<User>();
    
            return createdUser;
        }

        public Task<List<ElevOversigtDTO>> GetElevOversigt()
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _client.GetFromJsonAsync<List<User>>($"{serverUrl}/users");
        }
        
        public async Task<List<User>> GetAllUsersWithOutCurrent(int userId)
        {
            return await _client.GetFromJsonAsync<List<User>>($"{serverUrl}/users/withoutmyself/{userId}");
        }

        public Task<List<User>> GetAllUsersByStudentId(List<int> studentIds)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUser(int userId)
        {
            await _client.DeleteAsync($"{serverUrl}/users/{userId}");
        }

        public async Task ChangeRolle(string newRolle, int userId)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/users/updaterolle/{userId}/{newRolle}", new{});
        }

        public async Task DeActivateUser(int userId)
        { 
            await _client.PutAsJsonAsync($"{serverUrl}/users/deactivate/{userId}", userId);
        }

        public async Task ActivateUser(int userId)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/users/activate/{userId}", userId);
        }

        public Task UpdateHotel(Hotel hotel, int userId)
        {
            throw new NotImplementedException();
        }

        public Task SaveStudentPlan(int studentId, Plan plan)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(int currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetStudentsForløb(int leaderId)
        {
            throw new NotImplementedException();
        }
    }

}