using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class ForløbTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
            
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    
        public List<GoalTemplate> Goals { get; set; }

    }

}