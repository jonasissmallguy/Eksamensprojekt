using Core;
using MongoDB.Driver;

namespace Server
{
    public interface IKursusRepository
    {
        //Retunerer alle kurser
        Task<List<Kursus>> GetAllCourses();
        //Retunerer et kursus, hvor kursusId = _id
        Task<Kursus> GetCourseById(int kursusId);
        //Fjerner en elev fra kursus efter studentId, hvor kursusCode = CourseCode og sætter Participants -1
        Task<Kursus> RemoveStudentFromCourse(int studentId, string kursusCode);
        //Færdiggørt et kursus efter kursusId = _id og sætter Status = Completed
        Task<Kursus> CompleteCourse(int kursusId);
        //Tilføjer en user til kursusId = _id
        Task<Kursus> AddStudentToCourse(User user, int kursusId);
        //Retunerer alle KursusTemplate
        Task<List<KursusTemplate>> GetAllTemplates();
        //Indsætter et kursus
        Task<Kursus> SaveCourse(Kursus kursus);
        //Retunerer alle kurser med Status = Active, StartDate under 90 dage fra dd., og Participants er under MaxParticipants
        Task<List<Kursus>> GetFutureCourses();
        //Retunerer kommende kurser for studentId = _id
        Task<List<Kursus>> GetFutureCourseByStudentId(int studentId);
        
        
    }
}