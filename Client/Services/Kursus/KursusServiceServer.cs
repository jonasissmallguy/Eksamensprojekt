using Core;
using System.Net.Http.Json;
using Core.DTO.Kursus;

namespace Client
{
    public class KursusServiceServer : IKursus
    {
        private readonly HttpClient _client;
        
        public KursusServiceServer(HttpClient http)
        {
            _client = http;
        }

        public async Task<List<Kursus>> GetAllCourses()
        {
            return await _client.GetFromJsonAsync<List<Kursus>>($"kursus");
        }
        public async Task<Kursus> GetCourseById(int kursusId)
        {
            return await _client.GetFromJsonAsync<Kursus>($"kursus/{kursusId}");
        }
        
        public async Task SaveCourse(KursusCreationDTO kursus)
        {
             await _client.PostAsJsonAsync($"kursus", kursus);
        }
        

        public async Task<Kursus> RemoveStudentFromCourse(int studentId, Kursus kursus)
        {
            var result = await _client.DeleteAsync($"kursus/removestudent/{studentId}/{kursus.CourseCode}");

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }
            return result.Content.ReadFromJsonAsync<Kursus>().Result;
        }

        public async Task CompleteCourse(int kursusId)
        {
            await _client.PutAsJsonAsync($"kursus/complete/{kursusId}", new{});
        }
        
        public async Task<List<KursusTemplate>> GetAllTemplates()
        {
            return await _client.GetFromJsonAsync<List<KursusTemplate>>($"kursus/templates");
        }

        public async Task<Kursus> AddStudentToCourse(KursusDeltagerListeDTO user, int kursusId)
        {
          var result =  await _client.PutAsJsonAsync($"kursus/addstudent/{kursusId}", user);

          if (!result.IsSuccessStatusCode)
          {
              return null;
          }
          return result.Content.ReadFromJsonAsync<Kursus>().Result;
        }

        public async Task<List<KursusKommendeDTO>> GetFutureCourses()
        {
            return await _client.GetFromJsonAsync<List<KursusKommendeDTO>>($"kursus/nextup");
        }

        public async Task<List<KursusKommendeDTO>> GetFutureCoursesByStudentId(int studentId)
        {
            return await _client.GetFromJsonAsync<List<KursusKommendeDTO>>($"kursus/nextup/{studentId}");
        }
    }
}