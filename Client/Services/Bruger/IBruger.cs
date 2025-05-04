using Core;

namespace Client
{

    public interface IBruger
    {
        Task<BrugerProfilDTO> GetBrugerById(int userId);
        
        Task<bool> OpdaterBruger(int userId, BrugerProfilDTO updateBruger);

    }
}
