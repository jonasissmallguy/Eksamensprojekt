using Core;
using MongoDB.Driver;
using DotNetEnv;

namespace Server
{
    public class KursusRepository : IKursusRepository
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Kursus> _collection;
        private IMongoCollection<KursusTemplate> _collectionTemplate;

        public KursusRepository()
        {
            Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("comwell");
            _collection = _database.GetCollection<Kursus>("kurser");
            _collectionTemplate = _database.GetCollection<KursusTemplate>("kursertemplate");
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
            kursus.Id = 9996;
            _collection.InsertOne(kursus);
        }
    }
}