using Core;
using Microsoft.AspNetCore.Mvc;
using Server;

namespace Server
{

    [ApiController]
    [Route("hotels")]
    public class HotelController : ControllerBase
    {
        private IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }
        
        /// <summary>
        /// Metode til at oprette et nyt hotel
        /// </summary>
        /// <param name="newHotel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostHotel(HotelCreationDTO newHotel)
        {
            var hotel = new Hotel
            {
                HotelNavn = newHotel.HotelNavn,
                Address = newHotel.Address,
                Zip = newHotel.Zip,
                City = newHotel.City,
                Region = newHotel.Region
            };
            
            await _hotelRepository.SaveHotel(hotel);

            return Ok(hotel);
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotel = await _hotelRepository.GetAllHotels();
            return Ok(hotel);
        }
        
        
        
    }

}