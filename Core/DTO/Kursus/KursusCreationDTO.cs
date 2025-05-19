using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Kursus;

public class KursusCreationDTO
{
    [Required(ErrorMessage = "Venligst vælg et kursus")]
    public int? TemplateId { get; set; }
    public string Title { get; set; }
    
    [Required (ErrorMessage = "Venligst indtast en lokation")]
    public string Location { get; set; }
    
    [Required (ErrorMessage = "Venligst vælg en startdato")]
    public DateTime? StartDate { get; set; }
    
    [Required (ErrorMessage = "Venligst vælg en slutdato")]
    public DateTime? EndDate { get; set; }
    
    [Required (ErrorMessage = "Venligst udfyld en beskrivelse")]
    public string Description { get; set; }
}