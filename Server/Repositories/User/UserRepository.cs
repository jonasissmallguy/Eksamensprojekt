using Core;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace Server
{

    public class UserRepository : IUserRepository
    {
        private IMongoClient _userClient;
        private IMongoDatabase _userDatabase;
        private IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<BsonDocument> _countersCollection;


        public UserRepository()
        {
            string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

            _userClient = new MongoClient(ConnectionString);
            _userDatabase = _userClient.GetDatabase("comwell");
            _userCollection = _userDatabase.GetCollection<User>("users");
            _countersCollection = _userDatabase.GetCollection<BsonDocument>("counters"); 

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


        public async Task<User> GetUserById(int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllActiveUsers()
        {
            var filter = Builders<User>.Filter.Eq("Status", "Active");
            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<List<User>> GetAllUsersWithOutMyself(int userId)
        {
            var filter = Builders<User>.Filter.Ne("_id", userId);
            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq("Email", email);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }


        public async Task<User> SaveBruger(User bruger)
        {
            int id = await GetNextSequenceValue("userId");
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

        public async Task<DeleteResult> DeleteUser(int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            return await _userCollection.DeleteOneAsync(filter);
        }

        public async Task<UpdateResult> DeactivateUser(int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var update = Builders<User>.Update.Set("Status", "Deactivated");
            
            return await _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> ActivateUser(int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var update = Builders<User>.Update.Set("Status", "Active");
            
            return await _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateRolle(string rolle, int userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var update = Builders<User>.Update.Set("Rolle", rolle);
            
            return await _userCollection.UpdateOneAsync(filter, update);
        }
        
        public async Task<UpdateResult> UpdatePassword(string email, string updatedPassword)
        {
            var filter = Builders<User>.Filter.Eq("Email", email);
            var update = Builders<User>.Update.Set("Password", updatedPassword);
            
            return await _userCollection.UpdateOneAsync(filter, update);
        }
        

        public async Task<UpdateResult> UpdateHotel(int userId, int hotelId, string updatedHotelNavn)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var update = Builders<User>.Update
                .Set("HotelId", hotelId)
                .Set("HotelNavn", updatedHotelNavn);
            
            return await  _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<User>> GetAllStudents()
        {
            var filter = Builders<User>.Filter.Eq("Rolle", "Elev");
            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<List<User>> GetAllStudentsByHotelId(int hotelId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq("HotelId", hotelId),
                Builders<User>.Filter.Eq("Rolle", "Elev"));
            
             return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<List<User>> GetAllStudentsMissingCourse(string kursusCode)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Rolle, "Elev"),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.And(
                            Builders<Goal>.Filter.Eq(g => g.Type, "Kursus"),
                            Builders<Goal>.Filter.Eq(g => g.CourseCode, kursusCode),
                            Builders<Goal>.Filter.Eq(g => g.Status, "Active") 
                        )
                    )
                )
            );
            
            return await _userCollection.Find(filter).ToListAsync();
            
        }
    }
}

