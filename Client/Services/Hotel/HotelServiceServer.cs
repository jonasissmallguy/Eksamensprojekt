using System.Net.Http.Json;
using Core;

namespace Client
{

    public class HotelServiceServer : IHotel
    {
        private HttpClient _client;

        public HotelServiceServer(HttpClient client)
        {
            _client = client;
        }
        

        public async Task<List<Hotel>> GetHotels()
        {
            try
            {
               return await _client.GetFromJsonAsync<List<Hotel>>($"hotels");
            }

            catch (HttpRequestException)
            {
                return null;
            }
        }   

        public async Task CreateHotel(HotelCreationDTO newHotel)
        { 
            var hotel = await _client.PostAsJsonAsync($"hotels", newHotel);

            if (!hotel.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke oprette et hotel, prøv venligst igen");
            }
        }
    }

}