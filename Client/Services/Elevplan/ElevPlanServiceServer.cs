using System.Net.Http.Json;
using Core;

namespace Client
{

    public class ElevPlanServiceServer : IElevPlan
    {
        
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client = new();

        public ElevPlanServiceServer(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<Plan> CreateElevPlan(int studentId)
        {
            var response = await _client.PostAsync($"{serverUrl}/elevplan?studentId={studentId}", null);
            response.EnsureSuccessStatusCode(); 
            
            Plan plan = await response.Content.ReadFromJsonAsync<Plan>();
            return plan;
        }

        public Task SavePlan(Plan plan)
        {
            throw new NotImplementedException();
        }

        public Task<Plan> GetPlanByStudentId(int studentId)
        {
            throw new NotImplementedException();
        }
    }

}