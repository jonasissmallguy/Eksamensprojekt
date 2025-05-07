using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Comment
    { 
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Text { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
    }
}