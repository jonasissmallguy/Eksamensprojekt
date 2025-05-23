using System.Net.Http.Json;
using Core;

namespace Client
{

    public class HotelServiceServer : IHotel
    {
        private string serverUrl = "https://elevportalapi.azurewebsites.net";
        private HttpClient _client;

        public HotelServiceServer(HttpClient client)
        {
            _client = client;
        }
        

        public async Task<List<Hotel>> GetHotels()
        {
            return await _client.GetFromJsonAsync<List<Hotel>>($"{serverUrl}/hotels");
        }   

        public async Task CreateHotel(HotelCreationDTO newHotel)
        { 
            await _client.PostAsJsonAsync($"{serverUrl}/hotels", newHotel);
        }
    }

}