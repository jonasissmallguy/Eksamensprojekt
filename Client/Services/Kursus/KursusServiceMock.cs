

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
            StartDate = new DateTime(2025, 7, 24, 10, 0, 0),
            EndDate = new  DateTime(2025, 7, 24, 15, 0, 0),
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

    public Task SaveCourse(KursusCreationDTO kursus)
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

    public async Task RemoveStudentFromCourse(int studentId, Kursus kursus)
    {
        var kursuset = _allCourses.FirstOrDefault(x => x.Id == kursus.Id);
        kursuset.Students.RemoveAll(x => x.Id == studentId);
    }

    public async Task CompleteCourse(Kursus kursus)
    {
        List<User> allParticipants = new();
        string _kursusnavn = kursus.Title;
        
        foreach (var x in kursus.Students)
        {
            
            allParticipants.Add(x);
        }

        foreach (var student in allParticipants)
        {
            var forløbs = student.ElevPlan.Forløbs;
            
            foreach (var forløb in forløbs)
            {
                var goal = forløb.Goals.FirstOrDefault(x => x.Title == _kursusnavn);
                if (goal != null)
                {
                    goal.Status = "Completed";
                }
            }
        }
    }

    public Task<List<KursusTemplate>> GetAllTemplates()
    {
        throw new NotImplementedException();
    }
}