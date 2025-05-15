using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core
{
    public class Kursus
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } 
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public string Status { get; set; } = "Active"; //Active, InProgress...
        public List<int> StudentIds { get; set; }
        

    }

}