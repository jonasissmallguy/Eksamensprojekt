using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{
    public class KursusRepository : IKursusRepository
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Kursus> _collection;
        private IMongoCollection<KursusTemplate> _collectionTemplate;
        private readonly IMongoCollection<BsonDocument> _countersCollection;


        public KursusRepository()
        {
            string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            
            _client = new MongoClient(ConnectionString);
            _database = _client.GetDatabase("comwell");
            _collection = _database.GetCollection<Kursus>("kurser");
            _collectionTemplate = _database.GetCollection<KursusTemplate>("kursertemplate");
            _countersCollection = _database.GetCollection<BsonDocument>("counters"); 
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

        public async Task<List<Kursus>> GetAllCourses()
        {
            var filter = Builders<Kursus>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<Kursus> GetCourseById(int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursusId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveStudentFromCourse(int studentId, int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursusId);
            var update = Builders<Kursus>.Update.PullFilter(k => k.Students, s => s.Id == studentId);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        
        //Denne skal sætte for alle brugere der er på kursuset!
        public async Task<bool> CompleteCourse(Kursus kursus)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursus.Id);
            var update = Builders<Kursus>.Update.Set("Status", "Completed");
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public Task AddStudentToCourse(User user, int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursusId);
            var update = Builders<Kursus>.Update.Push("Students", user);
            
            return _collection.UpdateOneAsync(filter, update);
        }

        public async Task AddStudentToCourse(int studentId, Kursus kursus)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursus.Id);
            var update = Builders<Kursus>.Update.Set("Students", studentId);
            
        }

        public async Task<List<KursusTemplate>> GetAllTemplates()
        {
            var filter = Builders<KursusTemplate>.Filter.Empty;
            
            return await _collectionTemplate.Find(filter).ToListAsync();
        }

        public async Task SaveCourse(Kursus kursus)
        {
            kursus.Id = await GetNextSequenceValue("kursusId");
            _collection.InsertOne(kursus);
        }

        public async Task<List<Kursus>> GetFutureCourses()
        {
            var cutOff = DateOnly.FromDateTime(DateTime.Now).AddDays(90);
            
            var filter1 = Builders<Kursus>.Filter.Lte("StartDate", cutOff);
            var filter2 = Builders<Kursus>.Filter.Eq("Status", "Active");
            var filter = Builders<Kursus>.Filter.And(filter1, filter2);
            
            return await _collection.Find(filter).ToListAsync();
        }
    }
}