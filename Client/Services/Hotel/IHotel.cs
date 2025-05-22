using Core;

namespace Client
{
    public interface IHotel
    {
        Task<List<Hotel>> GetHotels();
        
        Task CreateHotel(HotelCreationDTO newHotel);

    }

}