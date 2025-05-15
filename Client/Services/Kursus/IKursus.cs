using Core;

namespace Client
{
    public interface IKursus
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task AddCourse(Kursus kursus);
        Task UpdateCourse(Kursus kursus);
        Task DeleteCourse(Kursus kursus, int kursusId);
        Task StartCourse(Kursus kursus);
        Task RemoveStudentFromCourse(Kursus kursus, int studentId);

    }
}