using Core;
using MongoDB.Driver;

namespace Server
{
    public interface IKursusRepository
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task<Kursus> RemoveStudentFromCourse(int studentId, string kursusCode);
        Task<Kursus> CompleteCourse(int kursusId);
        Task<Kursus> AddStudentToCourse(User user, int kursusId);
        Task<List<KursusTemplate>> GetAllTemplates();
        Task<Kursus> SaveCourse(Kursus kursus);
        Task<List<Kursus>> GetFutureCourses();
        Task<List<Kursus>> GetFutureCourseByStudentId(int studentId);
        
        
    }
}