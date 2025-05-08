using System.ComponentModel.DataAnnotations;
namespace Core
{

    public class BrugerLoginDTO
    {
        
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Du skal udfylde en gyldig e-mail")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Du skal udfylde et password")]
        public string Password { get; set; }
        public string Rolle { get; set; }
        public string FirstName { get; set; }
    }

}