using Core;

namespace Client
{

    public interface IBruger
    {
        Task<User> GetBrugerById(int userId);
        Task<bool> OpdaterBruger(int userId, User updateBruger);
        Task<List<ManagerDTO>> GetAllManagers();
        Task<User> OpretBruger(BrugerCreateDTO nyBruger);
        Task<List<ElevOversigtDTO>> GetElevOversigt();
        Task<List<User>> GetAllUsers();
        Task<List<User>> GetAllUsersWithOutCurrent(int userId);
        Task<List<User>> GetAllUsersByStudentId(List<int> studentIds);
        Task DeleteUser(int userId);
        Task ChangeRolle(string newRolle, int userId);
        Task DeActivateUser(int userId);
        Task ActivateUser(int userId);
        Task UpdateHotel(Hotel hotel, int userId);
        Task SaveStudentPlan(int studentId, Plan plan);
        Task<User> GetUserById(int currentUserId);
    }
}
