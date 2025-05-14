namespace Core
{

    public class ElevOversigtDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int? HotelId { get; set; }
        public string Hotel { get; set; }
        public string Roller { get; set; }
        public string Ansvarlig  {get; set;}
        public string Year { get; set; }
        public int SkoleId { get; set; }
        public int UddannelseId { get; set; }

    }

}