using Core;
using Core.DTO.Kursus;

namespace Client
{
    public interface IKursus
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task<Kursus> SaveCourse(KursusCreationDTO kursus);
        
        Task<Kursus> RemoveStudentFromCourse(int studentId, Kursus kursus);

        Task<bool> CompleteCourse(int kursusId);
        
        Task<List<KursusTemplate>> GetAllTemplates();
        
        Task<Kursus> AddStudentToCourse(KursusDeltagerListeDTO user, int kursusId);
        
        Task<List<KursusKommendeDTO>> GetFutureCourses();
        Task<List<KursusKommendeDTO>> GetFutureCoursesByStudentId(int studentId);

    }
}