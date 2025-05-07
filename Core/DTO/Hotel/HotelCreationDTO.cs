using System.ComponentModel.DataAnnotations;

namespace Core
{

    public class HotelCreationDTO
    {
        [Required]
        public string HotelNavn { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int? Zip { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public int HrId { get; set; }
    }

}