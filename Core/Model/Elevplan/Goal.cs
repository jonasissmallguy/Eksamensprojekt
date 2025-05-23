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
        
        public string? Type { get; set; }  // Kursus, Skole, Kompetence
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Status { get; set; } = "Active";
        public string? Semester { get; set; }
        
        //Kompetence
        public int? StarterId { get; set; } 
        public string? StarterName { get; set; } = String.Empty;
        public DateTime? StartedAt { get; set; } //Starter sætter
        public int? ConfirmerId { get; set; }
        public string? ConfirmerName { get; set; } = String.Empty;
        public DateTime? ConfirmedAt { get; set; } //Kok sæltter / Leder
        public DateTime? CompletedAt { get; set; } //Leder sætter
        
        //Skole
        public string? SkoleNavn { get; set; }
        
        public List<Comment>? Comments { get; set; } = new List<Comment>();
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}       