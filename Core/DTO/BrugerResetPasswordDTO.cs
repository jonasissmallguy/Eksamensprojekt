using System.ComponentModel.DataAnnotations;

namespace Core
{

    public class BrugerResetPasswordDTO
    {
        [Required(ErrorMessage = "Du skal udfylde en gyldig e-mail")]
        public string Email { get; set; }

    }

}