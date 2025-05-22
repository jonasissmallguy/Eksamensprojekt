using Core;
using System.Net.Http.Json;
using Core.DTO.Kursus;

namespace Client
{
    public class KursusServiceServer : IKursus
    {
        private string serverUrl = "http://localhost:5075";
        private readonly HttpClient _client;
        

        public KursusServiceServer(HttpClient http)
        {
            _client = http;
        }

        public async Task<List<Kursus>> GetAllCourses()
        {
            return await _client.GetFromJsonAsync<List<Kursus>>($"{serverUrl}/kursus");
        }
        public async Task<Kursus> GetCourseById(int kursusId)
        {
            return await _client.GetFromJsonAsync<Kursus>($"{serverUrl}/kursus/{kursusId}");
        }
        
        public async Task SaveCourse(KursusCreationDTO kursus)
        {
             await _client.PostAsJsonAsync($"{serverUrl}/kursus", kursus);
        }
        

        public async Task RemoveStudentFromCourse(int studentId, Kursus kursus)
        {
            int kursusId = kursus.Id;
            await _client.DeleteAsync($"{serverUrl}/kursus/removestudent/{studentId}/{kursusId}");
        }

        public async Task CompleteCourse(Kursus kursus)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/kursus/complete", kursus);
        }
        
        public async Task<List<KursusTemplate>> GetAllTemplates()
        {
            return await _client.GetFromJsonAsync<List<KursusTemplate>>($"{serverUrl}/kursus/templates");
        }

        public async Task AddStudentToCourse(KursusDeltagerListeDTO user, int kursusId)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/kursus/addstudent/{kursusId}", user);
        }

        public async Task<List<KursusKommendeDTO>> GetFutureCourses()
        {
            return await _client.GetFromJsonAsync<List<KursusKommendeDTO>>($"{serverUrl}/kursus/nextup");
        }
    }
}