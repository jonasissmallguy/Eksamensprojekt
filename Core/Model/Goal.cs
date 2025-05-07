using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Goal
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Type { get; set; }  // Kursus, delmål, kompetence
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Active";
        //public bool IsTemplate { get; set; } = false;
        public int Semester { get; set; }
        
        public int? StarterId { get; set; } 
        public string StarterName { get; set; } = String.Empty;
        public List<Comment> Comments { get; set; } 
        public DateTime StartedAt { get; set; }
        public DateTime DeadLineAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public int SortOrder { get; set; }
    }
}       