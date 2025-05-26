

using Core;
using Core.DTO.Kursus;

namespace Client;

public class KursusServiceMock : IKursus
{

    private List<Kursus> _allCourses = new List<Kursus>
    {
        new Kursus
        {
            Id = 1,
            Title = "Bæredygtighed",
            Location = "Comwell Hovedkontor",
            StartDate = new DateOnly(2025, 7, 24),
            EndDate = new  DateOnly(2025, 7, 24),
            Students = new List<User>
            {
            }
            
        }
    };

    private IElevPlan _elevplan;

    public KursusServiceMock(IElevPlan elevPlan)
    {
        _elevplan = elevPlan;
    }
    
    public async Task<List<Kursus>> GetAllCourses()
    {
        return _allCourses;
    }

    public async Task<Kursus> GetCourseById(int kursusId)
    {
        var kursus = _allCourses.FirstOrDefault(x => x.Id == kursusId);
        return kursus;
    }

    public Task<Kursus> SaveCourse(KursusCreationDTO kursus)
    {
        throw new NotImplementedException();
    }

    public Task SaveCourse(Kursus kursus)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCourse(Kursus kursus)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCourse(Kursus kursus, int kursusId)
    {
        throw new NotImplementedException();
    }

    public Task StartCourse(Kursus kursus)
    {
        throw new NotImplementedException();
    }

    public async Task<Kursus> RemoveStudentFromCourse(int studentId, Kursus kursus)
    {
        var kursuset = _allCourses.FirstOrDefault(x => x.Id == kursus.Id);
        kursuset.Students.RemoveAll(x => x.Id == studentId);

        return kursuset;
    }

    public async Task<bool> CompleteCourse(int kursusId)
    {
        List<User> allParticipants = new();
        //string _kursusnavn = kursus.Title;
        /*
        foreach (var x in kursus.Students)
        {
            
            allParticipants.Add(x);
        }
        */

        foreach (var student in allParticipants)
        {
            var forløbs = student.ElevPlan.Forløbs;
            
            foreach (var forløb in forløbs)
            {
                var goal = forløb.Goals.FirstOrDefault(x => x.Title == x.Title);
                if (goal != null)
                {
                    goal.Status = "Completed";
                }
            }
        }

        return true;
    }

    public async Task<Kursus> AddStudentToCourse(int userId, string userName, string hotelName, int kursusId)
    {
        throw new NotImplementedException();
    }

    public Task<List<KursusTemplate>> GetAllTemplates()
    {
        throw new NotImplementedException();
    }

    public Task<Kursus> AddStudentToCourse(KursusDeltagerListeDTO user, int kursusId)
    {
        throw new NotImplementedException();
    }

    public Task<List<KursusKommendeDTO>> GetFutureCourses()
    {
        throw new NotImplementedException();
    }

    public Task<List<KursusKommendeDTO>> GetFutureCoursesByStudentId(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task AddStudentToCourse(User user, int kursusId)
    {
        throw new NotImplementedException();
    }
}