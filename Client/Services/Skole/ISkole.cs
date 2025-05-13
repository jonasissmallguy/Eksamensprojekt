using Core;

namespace Client
{
    public interface ISkole
    {
        Task<List<SkoleDTO>> GetAllSkoleNames();
        Task<List<Skole>> GetSkoler();
        
        Task CreateSkole(SkoleDTO newSkole);

    }

}