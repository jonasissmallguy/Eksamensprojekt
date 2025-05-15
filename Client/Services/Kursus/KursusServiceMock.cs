using Client;
using Core;

namespace Client;

public class KursusServiceMock : IKursus
{

    private List<Kursus> _allCourses = new List<Kursus>
    {
        new Kursus
        {
            Id = 1,
            Title = "ESG og Bæredygtighed",
            Location = "Comwell Hovedkontor",
            StartDate = new DateTime(2025, 7, 24, 10, 0, 0),
            EndDate = new  DateTime(2025, 7, 24, 15, 0, 0),
        }
    };
    
    public async Task<List<Kursus>> GetAllCourses()
    {
        return _allCourses;
    }

    public async Task<Kursus> GetCourseById(int kursusId)
    {
        var kursus = _allCourses.FirstOrDefault(x => x.Id == kursusId);
        return kursus;
    }

    public Task AddCourse(Kursus kursus)
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

    public Task RemoveStudentFromCourse(Kursus kursus, int studentId)
    {
        throw new NotImplementedException();
    }
}