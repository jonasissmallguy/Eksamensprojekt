using Core;
using System.Net.Http.Json;

namespace Client
{
    public class KursusServiceServer : IKursus
    {
        private readonly HttpClient _http;

        public KursusServiceServer(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Kursus>> GetAllCourses()
        {
            return await _http.GetFromJsonAsync<List<Kursus>>("kursus");
        }

        public async Task<Kursus> GetCourseById(int kursusId)
        {
            return await _http.GetFromJsonAsync<Kursus>($"kursus/{kursusId}");
        }

        public async Task AddCourse(Kursus kursus)
        {
            await _http.PostAsJsonAsync("kursus", kursus);
        }

        public async Task UpdateCourse(Kursus kursus)
        {
            await _http.PutAsJsonAsync("kursus", kursus);
        }

        public async Task DeleteCourse(Kursus kursus, int kursusId)
        {
            await _http.DeleteAsync($"kursus/{kursusId}");
        }

        public async Task StartCourse(Kursus kursus)
        {
            await _http.PutAsJsonAsync("kursus/start", kursus);
        }

        public async Task RemoveStudentFromCourse(int studentId, Kursus kursus)
        {
            var url = $"kursus/remove-student?studentId={studentId}&kursusId={kursus.Id}";
            await _http.PutAsync(url, null);
        }

        public async Task CompleteCourse(Kursus kursus)
        {
            await _http.PutAsJsonAsync("kursus/complete", kursus);
        }
    }
}