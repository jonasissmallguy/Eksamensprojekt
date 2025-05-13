using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Skole
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string SkoleNavn { get; set; }
        public string Adresse { get; set; }
        
    }
}