using MongoDB.Driver;
using Core;

namespace Server
{

    public interface IHotelRepository
    {
        //Henter alle hoteller
        Task<List<Hotel>> GetAllHotels();
        //Gemmer et hotel
        Task SaveHotel(Hotel hotel);
        //Henter et hotel efter hotelId = _id
        Task<Hotel> GetHotelById(int hotelId);
        //Opdater hotel med KøkkenChefId og KøkkenChefNavn
        Task<UpdateResult> UpdateHotelChef(Hotel hotel);
        //Fjerner en køkkenchef fra Hotel hvor køkkenChefId = KøkkenChefId
        Task<UpdateResult> RemoveChefFromHotel(int køkkenChefId);
        

    }

}