using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class BrugerCreateDTO
    {
        [Required(ErrorMessage = "Venligst indtast et fornavn")]
        public string FrontName;
        
        [Required(ErrorMessage = "Venligst indtast et efternavn")]
        public string LastName;
        
        [Required(ErrorMessage = "Venligst indtast en e-mail")]
        public string Email;
        
        [Required(ErrorMessage = "Venligst et mobilnummer")]
        [Range(8,8,ErrorMessage = "Skal ikke indeholde landekode")]
        public int? Mobile;
        
        [Required(ErrorMessage = "Venligst indtast en rolle")]
        public string Rolle;
        
        [Required(ErrorMessage = "Venligst indtast et hotel")]
        public string Hotel;
    }
}