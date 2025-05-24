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
            return await _client.GetFromJsonAsync<List<Hotel>>($"hotels");
        }   

        public async Task CreateHotel(HotelCreationDTO newHotel)
        { 
            await _client.PostAsJsonAsync($"hotels", newHotel);
        }
    }

}