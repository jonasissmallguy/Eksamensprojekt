using Core;

namespace Client
{
    public interface IHotel
    {
        Task<List<HotelNameDTO>> GetAllHotelNames();
        Task<List<Hotel>> GetHotels();
        
        Task CreateHotel(HotelCreationDTO newHotel);

    }

}