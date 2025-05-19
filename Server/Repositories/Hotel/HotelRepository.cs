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