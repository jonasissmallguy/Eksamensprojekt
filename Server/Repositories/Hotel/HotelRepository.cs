using Core;
using Server;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{

    public class HotelRepository : IHotelRepository
    {
        private IMongoClient _hotelClient;
        private IMongoDatabase _hotelDatabase;
        private IMongoCollection<Hotel> _hotelCollection;
        private readonly IMongoCollection<BsonDocument> _countersCollection;

        public HotelRepository()
        {
            string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            
            _hotelClient = new MongoClient(ConnectionString);
            _hotelDatabase = _hotelClient.GetDatabase("comwell");
            _hotelCollection = _hotelDatabase.GetCollection<Hotel>("hotels");
            _countersCollection = _hotelDatabase.GetCollection<BsonDocument>("counters"); 

        }
        
        public async Task<int> GetNextSequenceValue(string sequenceName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", sequenceName);
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var result = await _countersCollection.FindOneAndUpdateAsync(filter, update, options); 
            return result["seq"].AsInt32;
        }

        
        
        public async Task<List<Hotel>> GetAllHotels()
        {
            var filter = Builders<Hotel>.Filter.Empty;
            return await _hotelCollection.Find(filter).ToListAsync();
        }

        public async Task SaveHotel(Hotel hotel)
        {
            int id = await GetNextSequenceValue("hotelId");
            hotel.Id = id;
            
            await _hotelCollection.InsertOneAsync(hotel);
        }

        public async Task<Hotel> GetHotelById(int id)
        {
            var filter = Builders<Hotel>.Filter.Eq("_id", id);
            return await _hotelCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> UpdateHotelChef(Hotel hotel)
        {
            var filter = Builders<Hotel>.Filter.Eq("_id", hotel.Id);
            var update = Builders<Hotel>.Update
                .Set("KøkkenChefId", hotel.KøkkenChefId)
                .Set("KøkkenChefNavn", hotel.KøkkenChefNavn);

            return await _hotelCollection.UpdateOneAsync(filter, update);
        }
    }

}