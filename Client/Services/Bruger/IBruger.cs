using Core;

namespace Client
{

    public interface IBruger
    {
        Task<User> GetBrugerById(int userId);
        Task<User> OpretBruger(BrugerCreateDTO nyBruger);
        Task<List<ElevOversigtDTO>> GetElevOversigt();
        Task<List<BrugerLoginDTO>> GetAllActiveUsers();
        Task<List<BrugerAdministrationDTO>> GetAllUsersWithOutCurrent(int userId);
        Task<bool> DeleteUser(int userId, string rolle);
        Task ChangeRolle(string newRolle, int userId);
        Task DeActivateUser(int userId, string rolle);
        Task ActivateUser(int userId);
        Task<bool> UpdateHotel(int hotelId, string hotelName, int userId);
        Task<List<KursusDeltagerListeDTO>> GetAllStudents();
        Task<bool> SendEmail(HashSet<int> studentIds, string email);

    }
}
