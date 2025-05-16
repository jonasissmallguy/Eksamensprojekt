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

        public HotelRepository()
        {
            Env.Load();
            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            
            _hotelClient = new MongoClient(ConnectionString);
            _hotelDatabase = _hotelClient.GetDatabase("comwell");
            _hotelCollection = _hotelDatabase.GetCollection<Hotel>("hotels");
        }

        public async Task<int> GetNextId()
        {
            long count = await _hotelCollection.CountDocumentsAsync(new BsonDocument());
            return (int)count + 1;
        }
        
        
        public async Task<List<Hotel>> GetAllHotels()
        {
            var filter = Builders<Hotel>.Filter.Empty;
            return await _hotelCollection.Find(filter).ToListAsync();
        }

        public async Task SaveHotel(Hotel hotel)
        {
            int id = await GetNextId();
            hotel.Id = id;
            
            await _hotelCollection.InsertOneAsync(hotel);
        }
    }

}