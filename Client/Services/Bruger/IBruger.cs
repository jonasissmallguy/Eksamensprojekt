using Core;

namespace Client
{

    public interface IBruger
    {
        Task<BrugerProfilDTO> GetBrugerById(int userId);
        
        Task<bool> OpdaterBruger(int userId, BrugerProfilDTO updateBruger);

        Task<List<ManagerDTO>> GetAllManagers();
        
        Task<User> OpretBruger(BrugerCreateDTO nyBruger);

        Task<List<ElevOversigtDTO>> GetElevOversigt();

        Task <List<User>> GetAllUsers();

        Task<List<User>> GetAllUsersWithOutCurrent(int userId);
        
        Task<List<User>> GetAllUsersByStudentId(List<int> studentIds);
        
        Task DeleteUser(int userId);

        Task ChangeRolle(string newRolle, int userId);
        
        Task DeActivateUser(int userId);

        Task ActivateUser(int userId);
        
        Task UpdateHotel(Hotel hotel, int userId);

    }
}
