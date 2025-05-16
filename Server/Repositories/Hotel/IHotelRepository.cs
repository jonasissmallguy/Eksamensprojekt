using Core;

namespace Server
{

    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotels();
        Task SaveHotel(Hotel hotel);

    }

}