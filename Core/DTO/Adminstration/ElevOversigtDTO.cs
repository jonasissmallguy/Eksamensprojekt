namespace Core
{

    public class ElevOversigtDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int? HotelId { get; set; }
        public string HotelNavn { get; set; }
        public string Roller { get; set; }
        public string Ansvarlig  {get; set;}
        public string Year { get; set; }
        public string Skole { get; set; }
        public string Uddannelse { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

    }

}