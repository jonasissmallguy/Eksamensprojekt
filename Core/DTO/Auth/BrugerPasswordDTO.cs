using System.ComponentModel.DataAnnotations;

namespace Core
{

    public class BrugerPasswordDTO
    {
        [Required(ErrorMessage = "Venligst angiv en ny adgangskode")]
        [Length(8,20, ErrorMessage = "Din adgangskode skal være mellem 8 & 20 tegn")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Venligst angiv en ny adgangskode")]
        [Compare("Password", ErrorMessage = "De to adgangskoder er ikke ens")]
        public string ConfirmPassword { get; set; }

    }

}