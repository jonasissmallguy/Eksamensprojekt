using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Hotel
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string HotelNavn { get; set; }
        public string Address { get; set; }
        public int? Zip { get; set; }
        public string City { get; set; }
        public Region Region { get; set; } // hvad skal det her?
        public User KøkkenChef { get; set; } // hvad skal det her?
    }
}