using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class BrugerCreateDTO
    {
        
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast et fornavn")]
        public string FirstName;
        
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
        public int? HotelId;
        [Required(ErrorMessage = "Venligst indtast en startdato")]
        public DateOnly? StartDate { get; set; }

    }
}