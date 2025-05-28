using Core;
using Microsoft.AspNetCore.Mvc;

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
        /// Opretter et nyt hotel
        /// </summary>
        /// <param name="newHotel"></param>
        [HttpPost]
        public async Task<IActionResult> PostHotel(HotelCreationDTO newHotel)
        {
            
            //Check unique
            var unique = await _hotelRepository.CheckUnique(newHotel.HotelNavn);

            if (!unique)
            {
                return Conflict("Dette hotelnavn er ikke unikt");
            }
            
            var hotel = new Hotel
            {
                HotelNavn = newHotel.HotelNavn,
                Address = newHotel.Address,
                Zip = newHotel.Zip,
                City = newHotel.City,
                Region = newHotel.Region
            };
            
            await _hotelRepository.SaveHotel(hotel);

            return Ok();
        }

     
        /// <summary>
        /// Henter alle hoteller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelRepository.GetAllHotels();
            
            return Ok(hotels);
        }
        
        
        
    }

}