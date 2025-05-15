using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{

    public class TemplateRepository : ITemplateRepository
    {
        private IMongoClient _templateClient;
        private IMongoDatabase _templateDatabase;
        private IMongoCollection<PlanTemplate> _templateCollection;


        public TemplateRepository()
        {
            Env.Load();
            
            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            
            _templateClient = new MongoClient(ConnectionString);
            _templateDatabase = _templateClient.GetDatabase("comwell");
            _templateCollection = _templateDatabase.GetCollection<PlanTemplate>("plantemplate");
        }
        
        public async Task<PlanTemplate> GetPlanTemplate(int id)
        {
            var filter = Builders<PlanTemplate>.Filter.Eq("_id", id);
            return await _templateCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}