using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Region
    {
        [BsonId] 
        public ObjectId Id { get; set; }
        public string Navn { get; set; }
    }
}