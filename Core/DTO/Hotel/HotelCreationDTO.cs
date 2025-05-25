using System.ComponentModel.DataAnnotations;

namespace Core
{

    public class HotelCreationDTO
    {
        [Required(ErrorMessage = "Venligst intast et hotelnavn")]
        public string HotelNavn { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast en addresse")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast et postnummer")]
        public int? Zip { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast en by")]
        public string City { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast en region")]
        public string Region { get; set; }
        
    }
}