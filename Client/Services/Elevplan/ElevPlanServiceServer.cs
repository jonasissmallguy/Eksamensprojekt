using System.Net;
using System.Net.Http.Json;
using Core;

namespace Client
{

    public class ElevPlanServiceServer : IElevPlan
    {
        
        private HttpClient _client = new();

        public ElevPlanServiceServer(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<Plan> GetElevPlanTemplate(int studentId)
        {
            var response = await _client.GetAsync($"elevplan/gettemplate/{studentId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Plan>();

        }

        public async Task<bool> SaveElevPlan(Plan plan, int studentId)
        {
            var update = await _client.PostAsJsonAsync($"elevplan/{studentId}", plan);

            if (!update.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }
    }
   

}