using System.Net.Http.Json;
using Core;

namespace Client
{

    public class TempalteServiceServer : ITemplate
    {
        
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client = new();

        public TempalteServiceServer(HttpClient client)
        {
            _client = client;
        }
        
        public PlanTemplate CreateTemplate()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<int, Goal>> GetGoals()
        {
            throw new NotImplementedException();
        }

        public async Task<PlanTemplate> GetTemplateById(int templateId)
        {
            return await _client.GetFromJsonAsync<PlanTemplate>($"{serverUrl}/template/{templateId}");
        }

        public Task<List<PlanTemplate>> GetAllTemplates()
        {
            throw new NotImplementedException();
        }
    }

}