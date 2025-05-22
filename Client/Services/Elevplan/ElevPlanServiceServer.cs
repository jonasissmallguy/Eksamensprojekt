using System.Net;
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
            var response = await _client.PostAsJsonAsync($"{serverUrl}/elevplan/{studentId}", new { studentId });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Plan>();
            }
            
            throw new Exception($"Error creating elev plan: {response.StatusCode}");
        }
        
    }
   

}