using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Restaurant
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string RestaurantNavn { get; set; }
        public string Addresse { get; set; }
        public int Postnummer { get; set; }
        public string By { get; set; }
        public ObjectId RegionId { get; set; }

    }

}