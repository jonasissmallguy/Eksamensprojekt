using Core;

namespace Client
{
    public class HotelServiceMock : IHotel
    {
        List<Hotel> hotels = new List<Hotel>()
        {
            new Hotel
            {
                Id = 1,
                HotelNavn = "Comwell Aarhus"
            },
            new Hotel
            {
                Id = 2,
                HotelNavn = "Comwell Koldsborg"
            }
        };
        
        public async Task<List<HotelNameDTO>> GetAllHotelNames()
        {
            List<HotelNameDTO> hotelNames = new();

            foreach (Hotel hotel in hotels)
            {
                 hotelNames.Add(new HotelNameDTO
                 {
                     HotelName = hotel.HotelNavn
                 });
            }
            return hotelNames;
        }

        public Task<List<Hotel>> GetHotels()
        {
            throw new NotImplementedException();
        }
    }
}