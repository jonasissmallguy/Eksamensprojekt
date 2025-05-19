using Core;
using Core.DTO.Kursus;

namespace Client
{
    public interface IKursus
    {
        Task<List<Kursus>> GetAllCourses();
        Task<Kursus> GetCourseById(int kursusId);
        Task SaveCourse(KursusCreationDTO kursus);
        Task UpdateCourse(Kursus kursus);
        Task DeleteCourse(Kursus kursus, int kursusId);
        Task StartCourse(Kursus kursus);
        Task RemoveStudentFromCourse(int studentId, Kursus kursus);

        Task CompleteCourse(Kursus kursus);
        
        Task<List<KursusTemplate>> GetAllTemplates();
        
        Task AddStudentToCourse(int studentId, Kursus kursus);

    }
}