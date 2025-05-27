using Core;
using MongoDB.Driver;
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

        public async Task<Kursus> RemoveStudentFromCourse(int studentId, string kursusCode)
        {
            var filter = Builders<Kursus>.Filter.Eq("CourseCode", kursusCode);
            var update = Builders<Kursus>.Update.Combine(Builders<Kursus>.Update.PullFilter(k => k.Students, s => s.Id == studentId),
                Builders<Kursus>.Update.Inc("Participants", -1));
            
            var options = new FindOneAndUpdateOptions<Kursus>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);            
            return result;

        }
        
        public async Task<Kursus> CompleteCourse(int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursusId);
            var update = Builders<Kursus>.Update.Set("Status", "Completed");


            var updatedCourse = await _collection.FindOneAndUpdateAsync(filter, update);

            return updatedCourse;

        }


        public async Task<Kursus> AddStudentToCourse(User user, int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq("_id", kursusId);

            var update = Builders<Kursus>.Update.Combine(
                Builders<Kursus>.Update.Push("Students", user),
                Builders<Kursus>.Update.Inc("Participants", 1)
            );            
            var options = new FindOneAndUpdateOptions<Kursus>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);
            
            return result;
        }
        

        public
            async Task<List<KursusTemplate>> GetAllTemplates()
        {
            var filter = Builders<KursusTemplate>.Filter.Empty;
            
            return await _collectionTemplate.Find(filter).ToListAsync();
        }

        public async Task<Kursus> SaveCourse(Kursus kursus)
        {
            kursus.Id = await GetNextSequenceValue("kursusId");
            await _collection.InsertOneAsync(kursus);
            return kursus;
        }

        public async Task<List<Kursus>> GetFutureCourses()
        {
            var cutOff = DateOnly.FromDateTime(DateTime.Now).AddDays(90);
            
            var filter1 = Builders<Kursus>.Filter.Lte("StartDate", cutOff);
            var filter2 = Builders<Kursus>.Filter.Eq("Status", "Active");
            var filter3 = new BsonDocumentFilterDefinition<Kursus>(
                new BsonDocument("$expr", 
                    new BsonDocument("$lt", new BsonArray { "$Participants", "$MaxParticipants" })
                )
            ); 
            
            var filter = Builders<Kursus>.Filter.And(filter1, filter2,filter3);
            
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<Kursus>> GetFutureCourseByStudentId(int studentId)
        {
            var filter = Builders<Kursus>.Filter.ElemMatch(s => s.Students, s => s.Id == studentId);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}