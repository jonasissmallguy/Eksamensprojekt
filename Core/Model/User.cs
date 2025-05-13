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
        public int? HotelId { get; set; } 
        
        public string HotelName { get; set; }
        
        public DateOnly? StartDate { get; set; }
        public string Year { get; set; }
        public int SkoleId { get; set; }
    }
}