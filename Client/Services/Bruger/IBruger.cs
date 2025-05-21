using Core;

namespace Client
{

    public interface IBruger
    {
        Task<User> GetBrugerById(int userId);
        Task<bool> OpdaterBruger(int userId, User updateBruger);
        Task<User> OpretBruger(BrugerCreateDTO nyBruger);
        Task<List<ElevOversigtDTO>> GetElevOversigt();
        Task<List<BrugerLoginDTO>> GetAllUsers();
        Task<List<BrugerLoginDTO>> GetAllActiveUsers();
        Task<List<BrugerAdministrationDTO>> GetAllUsersWithOutCurrent(int userId);
        Task<List<User>> GetAllUsersByStudentId(List<int> studentIds);
        Task<bool> DeleteUser(int userId, string rolle);
        Task ChangeRolle(string newRolle, int userId);
        Task DeActivateUser(int userId, string rolle);
        Task ActivateUser(int userId);
        Task<bool> UpdateHotel(int hotelId, string hotelName, int userId);
        Task SaveStudentPlan(int studentId, Plan plan);
        Task<User> GetUserById(int currentUserId);
        
        Task<List<KursusDeltagerListeDTO>> GetAllStudents();

        Task<bool> SendEmail(HashSet<int> studentIds);

    }
}
