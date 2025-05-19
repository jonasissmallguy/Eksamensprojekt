using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class BrugerCreateDTO
    {
        
        public int Id { get; set; }
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }
        
        public int? Mobile { get; set; }
        
        public string Rolle { get; set; }
        
        public string Køn { get; set; }
        
        public int HotelId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Year { get; set; }
        public string Skole { get; set; }
        public string Uddannelse { get; set; }
        

    }
}