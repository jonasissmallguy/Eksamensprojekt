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

        public KursusRepository()
        {
            Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("comwell");
            _collection = _database.GetCollection<Kursus>("kurser");
        }

        public async Task<List<Kursus>> GetAllCourses()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Kursus> GetCourseById(int kursusId)
        {
            return await _collection.Find(k => k.Id == kursusId).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveStudentFromCourse(int studentId, int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq(k => k.Id, kursusId);
            var update = Builders<Kursus>.Update.PullFilter(k => k.Students, s => s.Id == studentId);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> CompleteCourse(Kursus kursus)
        {
            var filter = Builders<Kursus>.Filter.Eq(k => k.Id, kursus.Id);
            var update = Builders<Kursus>.Update.Set("Status", "Completed");
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}