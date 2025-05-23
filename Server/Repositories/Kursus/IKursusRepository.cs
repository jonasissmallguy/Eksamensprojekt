using Core;

namespace Server
{
    public interface IKursusRepository
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task<bool> RemoveStudentFromCourse(int studentId, int kursusId);
        Task<bool> CompleteCourse(Kursus kursus);
        
        Task AddStudentToCourse(User user, int kursusId);
        
        Task<List<KursusTemplate>> GetAllTemplates();
        Task SaveCourse(Kursus kursus);
        Task<List<Kursus>> GetFutureCourses();
        Task<List<Kursus>> GetFutureCourseByStudentId(int studentId);
        
        
    }
}