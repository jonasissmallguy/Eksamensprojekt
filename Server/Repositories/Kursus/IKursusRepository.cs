using Core;

namespace Server
{
    public interface IKursusRepository
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task<bool> RemoveStudentFromCourse(int studentId, int kursusId);
        Task<bool> CompleteCourse(Kursus kursus);
    }
}