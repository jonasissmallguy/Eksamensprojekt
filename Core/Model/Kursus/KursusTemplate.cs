using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class KursusTemplate
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    
    public int? Id { get; set; }
    public string Title { get; set; }
}