using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class BrugerCreateDTO
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage = "Venligst indtast et fornavn")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast et efternavn")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast en e-mail")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Venligst et mobilnummer")]
        public int? Mobile { get; set; }
        
        [Required(ErrorMessage = "Venligst indtast en rolle")]
        public string Rolle { get; set; }
        
        [Required(ErrorMessage = "Venligst angiv et køn")]
        public string Køn { get; set; }
        
        //[Required(ErrorMessage = "Venligst indtast et hotel")]
        //public int HotelId { get; set; }
        [Required(ErrorMessage = "Venligst indtast en startdato")]
        public DateOnly? StartDate { get; set; }
        [Required(ErrorMessage = "Venligst indtast en aflslutningsdato")]
        public DateOnly? EndDate { get; set; }
        [Required(ErrorMessage = "Venligst indtast et semester år")]
        public string Year { get; set; }
        [Required(ErrorMessage = "Venligst indtast en skole")]
        public string Skole { get; set; }
        [Required(ErrorMessage = "Venligst indtast forløb")]
        public string Uddannelse { get; set; }

    }
}