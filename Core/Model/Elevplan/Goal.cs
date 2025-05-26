    using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Goal
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        
        public int ForløbId { get; set; }
        
        public int PlanId { get; set; }
  
        public string? Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Status { get; set; } = "Active";

        [BsonIgnoreIfNull]
        public int? StarterId { get; set; } 
        [BsonIgnoreIfNull]
        public string? StarterName { get; set; }
        [BsonIgnoreIfNull]
        public DateTime? StartedAt { get; set; }
        [BsonIgnoreIfNull]
        public int? ConfirmerId { get; set; }
        [BsonIgnoreIfNull]
        public string? ConfirmerName { get; set; }
        [BsonIgnoreIfNull]
        public DateTime? ConfirmedAt { get; set; } 
        [BsonIgnoreIfNull]
        public DateTime? CompletedAt { get; set; } 
        [BsonIgnoreIfNull]
        public string? SkoleNavn { get; set; }

        [BsonIgnoreIfNull] public List<Comment> Comments { get; set; } = new();
        [BsonIgnoreIfNull]
        public DateTime? StartDate { get; set; }
        [BsonIgnoreIfNull]
        public DateTime? EndDate { get; set; }
        [BsonIgnoreIfNull]
        public string? CourseCode { get; set; }

    }
}       