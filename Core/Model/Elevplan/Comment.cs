using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Comment
    { 
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public int PlanId { get; set; } //hvorfor?
        public int ForløbId { get; set; } //hvorfor?
        public int GoalId { get; set; }  //hvorfor?
        public string Text { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        
    }
}