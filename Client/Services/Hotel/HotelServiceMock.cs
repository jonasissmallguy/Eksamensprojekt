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
                     // Tilføjet af Rasmus
                     HotelId = hotel.Id,
                     HotelName = hotel.HotelNavn
                 });
            }
            return hotelNames;
        }

        public Task<List<Hotel>> GetHotels()
        {
            throw new NotImplementedException();
        }

        public int GenerateHotelId()
        {
            Random random = new();
            var id = random.Next(1,9999);
            return id;
        }

        public async Task CreateHotel(HotelCreationDTO newHotel)
        {
            var hotel = new Hotel
            {
                Id = GenerateHotelId(),
                HotelNavn = newHotel.HotelNavn,
                Address = newHotel.Address,
                Zip = newHotel.Zip,
                City = newHotel.City,
            };
            hotels.Add(hotel);
        }
    }
}