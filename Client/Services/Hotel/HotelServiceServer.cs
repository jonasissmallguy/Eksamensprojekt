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
               var result = await _client.GetFromJsonAsync<List<Hotel>>($"hotels");
               
               return result ?? new List<Hotel>();
            }

            catch (HttpRequestException)
            {
                return new List<Hotel>();
            }
        }   

        public async Task CreateHotel(HotelCreationDTO newHotel)
        { 
            var response = await _client.PostAsJsonAsync($"hotels", newHotel);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
    }

}