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
        private readonly IMongoCollection<BsonDocument> _countersCollection;


        public ElevplanRepository()
        {
            Env.Load();
            
            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            
            _elevplanClient = new MongoClient(ConnectionString);
            _elevplanDatabase = _elevplanClient.GetDatabase("comwell");
            _elevPlanCollection = _elevplanDatabase.GetCollection<User>("users");
            _countersCollection = _elevplanDatabase.GetCollection<BsonDocument>("counters"); 
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
        
        public async Task<UpdateResult> SaveElevplan(int studentId, Plan plan)
        {
            plan.Id = await GetNextSequenceValue("elevPlanId");
            plan.StudentId = studentId;

            foreach (var forløb in plan.Forløbs)
            {
                forløb.Id = await GetNextSequenceValue("forløbId");

                foreach (var goal in forløb.Goals)
                {
                    goal.Id = await GetNextSequenceValue("goalId");
                    goal.ForløbId = forløb.Id;
                    goal.PlanId = plan.Id;
                }
            }

            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update.Set("ElevPlan", plan);

            return await _elevPlanCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Plan> GetPlanByStudentId(int studentId)
        {
            var user = await _elevPlanCollection.Find(Builders<User>.Filter.Eq("_id", studentId)).FirstOrDefaultAsync();
            return user.ElevPlan;
        }
    }

}