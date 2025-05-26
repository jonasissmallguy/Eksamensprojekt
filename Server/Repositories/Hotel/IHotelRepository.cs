using MongoDB.Driver;
using Core;

namespace Server
{

    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotels();
        Task SaveHotel(Hotel hotel);
        Task<Hotel> GetHotelById(int id);
        
        Task<UpdateResult> UpdateHotelChef(Hotel hotel);
        
        Task<UpdateResult> RemoveManagerFromHotel(int køkkenChefId);
        

    }

}