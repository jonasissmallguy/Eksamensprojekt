using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class GoalTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public List<CommentTemplate> Comments { get; set; }
        
    }
}