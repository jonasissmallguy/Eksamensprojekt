using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class CommentTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        public string Text { get; set; }
        public int AnsvarligId { get; set; }
        public string AnsvarligName { get; set; }
    }
}