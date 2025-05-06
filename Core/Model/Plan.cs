using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Plan
    {
        [BsonId]
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Goal> Goals { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}