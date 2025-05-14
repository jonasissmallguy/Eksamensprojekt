using Core;

namespace Client
{
    public interface IUddannelse
    {
        Task<List<UddannelseDTO>> GetAllUddannelseNames();
        Task<List<Uddannelse>> GetUddannelser();
        
        Task CreateUddannelse(UddannelseDTO newUddannelse);

    }

}