using System.ComponentModel.DataAnnotations;


namespace Core
{

    public class BrugerResetKodeDTO
    {
        [Required(ErrorMessage = "Du skal udfylde den 8-cifrede kode")]
        [Length(8,8, ErrorMessage = "Du skal udfylde den 8-cifrede kode")]
        public string Kode { get; set; }
    }

}