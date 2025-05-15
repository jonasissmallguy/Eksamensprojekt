using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{

    public class ElevplanRepository : IElevplan
    {
        
        private IMongoClient _elevplanClient;
        private IMongoDatabase _elevplanDatabase;
        private IMongoCollection<User> _elevPlanCollection;


        public ElevplanRepository()
        {
            Env.Load();
            
            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            
            _elevplanClient = new MongoClient(ConnectionString);
            _elevplanDatabase = _elevplanClient.GetDatabase("comwell");
            _elevPlanCollection = _elevplanDatabase.GetCollection<User>("users");
        }
        
        public async Task<UpdateResult> SaveElevplan(int studentId, Plan plan)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update.Set("ElevPlan", plan);
            
            return await _elevPlanCollection.UpdateOneAsync(filter, update);
            
        }
    }

}