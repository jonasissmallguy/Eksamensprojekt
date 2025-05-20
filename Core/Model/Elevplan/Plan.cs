using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Plan
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Forløb> Forløbs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public string Status { get; set;  }
        
        public int TotalGoals { get; set; }
        public int CompletedGoals { get; set; }
        
    }

}