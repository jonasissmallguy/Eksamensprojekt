using System.ComponentModel.DataAnnotations;


namespace Core
{

    public class BrugerResetKodeDTO
    {
        [Required(ErrorMessage = "Du skal udfylde den 6-cifrede kode")]
        public string Kode { get; set; }

    }

}