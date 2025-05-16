using Client;
using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;


namespace Server
{
    public class KursusRepository : IKursusRepository
    {
        private IMongoClient _kursusClient;
        private IMongoDatabase _kursusDatabase;
        private IMongoCollection<Kursus> _kursusCollection;

        public KursusRepository()
        {
            Env.Load();

            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            _kursusClient = new MongoClient(connectionString);
            _kursusDatabase = _kursusClient.GetDatabase("comwell");
            _kursusCollection = _kursusDatabase.GetCollection<Kursus>("kurser");
        }

        public async Task<List<Kursus>> GetAllCourses()
        {
            return await _kursusCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Kursus> GetCourseById(int kursusId)
        {
            return await _kursusCollection.Find(k => k.Id == kursusId).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveStudentFromCourse(int studentId, int kursusId)
        {
            var filter = Builders<Kursus>.Filter.Eq(k => k.Id, kursusId);

            var update = Builders<Kursus>.Update.PullFilter(
                k => k.Students,
                Builders<User>.Filter.Eq(u => u.Id, studentId)
            );

            var result = await _kursusCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> CompleteCourse(Kursus kursus)
        {
            var filter = Builders<Kursus>.Filter.Eq(k => k.Id, kursus.Id);
            var update = Builders<Kursus>.Update.Set("Status", "Completed");

            var result = await _kursusCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
