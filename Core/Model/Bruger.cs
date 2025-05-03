
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core
{

    public class Bruger
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public string Rolle { get; set; } 
        public string? RestaurantId { get; set; } 
        public string? MentorId { get; set; }
        public List<ObjectId?> KokkeeleverIds { get; set; }

    }

}