using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core
{
    public class User
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; } 
        public int? Mobile { get; set; }
        public string Email { get; set; }
        public string Rolle { get; set; } 
        public int HotelId { get; set; }
        public string HotelNavn { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Year { get; set; }
        public string Status { get; set; } = "Active";
        public Plan ElevPlan { get; set; }
        
        public string Skole { get; set; }
        
        public string Uddannelse { get; set; }
        public int ForløbId { get; set; }
        
    }
}