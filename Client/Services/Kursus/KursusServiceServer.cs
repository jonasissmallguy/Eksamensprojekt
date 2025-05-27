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
            try
            {
                var result = await _client.GetFromJsonAsync<List<Kursus>>($"kursus");
                
                return result ?? new List<Kursus>();
                
            }
            catch (HttpRequestException)
            {
                return new List<Kursus>();
            }
        }
        public async Task<Kursus> GetCourseById(int kursusId)
        {
            try
            {
                return await _client.GetFromJsonAsync<Kursus>($"kursus/{kursusId}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
        
        public async Task<Kursus> SaveCourse(KursusCreationDTO kursus)
        {
             var result = await _client.PostAsJsonAsync($"kursus", kursus);

             if (!result.IsSuccessStatusCode)
             {
                 return null;
             }
             return await result.Content.ReadFromJsonAsync<Kursus>();
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

        public async Task<bool> CompleteCourse(int kursusId)
        {
            var result = await _client.PutAsJsonAsync($"kursus/complete/{kursusId}", new{});

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        
        public async Task<List<KursusTemplate>> GetAllTemplates()
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<KursusTemplate>>($"kursus/templates");
                
                return result ?? new List<KursusTemplate>();
                
            }
            catch (HttpRequestException)
            {
                return null;
            }
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
            try
            {
                return await _client.GetFromJsonAsync<List<KursusKommendeDTO>>($"kursus/nextup");
            }
            catch (HttpRequestException)
            {
                return new List<KursusKommendeDTO>();
            }
        }

        public async Task<List<KursusKommendeDTO>> GetFutureCoursesByStudentId(int studentId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<KursusKommendeDTO>>($"kursus/nextup/{studentId}");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Kunne ikke finde data");
                return new List<KursusKommendeDTO>();
            }
        }
    }
}