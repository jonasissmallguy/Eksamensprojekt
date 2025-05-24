using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public int? Mobile { get; set; }

        public string Email { get; set; }

        public string Rolle { get; set; }  // "Elev", "HR", etc.

        public int HotelId { get; set; }

        public string HotelNavn { get; set; }

        [BsonIgnoreIfNull]
        public DateOnly? StartDate { get; set; }

        [BsonIgnoreIfNull]
        public DateOnly? EndDate { get; set; }

        [BsonIgnoreIfNull]
        public string Year { get; set; }

        public string Status { get; set; } = "Active";

        [BsonIgnoreIfNull]
        public Plan? ElevPlan { get; set; }

        [BsonIgnoreIfNull]
        public string? Skole { get; set; }

        [BsonIgnoreIfNull]
        public string? Uddannelse { get; set; }
    }
}