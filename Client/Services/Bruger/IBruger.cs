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


    }
}
