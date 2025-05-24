using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core
{
    public class Kursus
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; } = "Active"; //Active, InProgress...
        public string Description { get; set; }
        public List<User> Students { get; set; } = new List<User>();
        public int Participants { get; set; }
        public int MaxParticipants { get; set; }
    }

}