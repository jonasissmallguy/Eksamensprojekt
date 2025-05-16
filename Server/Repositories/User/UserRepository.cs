using Core;
using DotNetEnv;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Server
{

    public class UserRepository : IUserRepository
    {
        private IMongoClient _userClient;
        private IMongoDatabase _userDatabase;
        private IMongoCollection<User> _userCollection;

        public UserRepository()
        {
            Env.Load();
            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");

            _userClient = new MongoClient(ConnectionString);
            _userDatabase = _userClient.GetDatabase("comwell");
            _userCollection = _userDatabase.GetCollection<User>("users");
        }

        public async Task<int> GetNextId() //Hjælpefunktion til at få fortløbende id
        {
            long count = await _userCollection.CountDocumentsAsync(new BsonDocument());
            return (int)count + 1;
        }


        public async Task<User> GetUserById(int id)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllUsersWithOutMyself(int userId)
        {
            var filter = Builders<User>.Filter.Ne("_id", userId);
            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<User> SaveBruger(User bruger)
        {
            int id = await GetNextId();
            bruger.Id = id;

            await _userCollection.InsertOneAsync(bruger);

            return bruger;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var filter = Builders<User>.Filter.Empty;

            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> CheckUnique(string email)
        {

            var filter = Builders<User>.Filter.Eq("Email", email);

            if (_userCollection.Find(filter).Any())
            {
                return false;
            }

            return true;
        }

        public Task DeleteUser(int studentId)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            return _userCollection.DeleteOneAsync(filter);
        }

        public async Task DeactivateUser(int studentId)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update.Set("Status", "Deactivated");
            
            await _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task ActivateUser(int studentId)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update.Set("Status", "Active");
            
            await _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateRolle(string rolle, int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var update = Builders<User>.Update.Set("Rolle", rolle);
            
            await _userCollection.UpdateOneAsync(filter, update);
            
        }
        
    }
}

