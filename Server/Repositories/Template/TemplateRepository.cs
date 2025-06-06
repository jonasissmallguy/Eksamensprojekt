﻿using Core;
using MongoDB.Driver;

namespace Server
{

    public class TemplateRepository : ITemplateRepository
    {
        private IMongoClient _templateClient;
        private IMongoDatabase _templateDatabase;
        private IMongoCollection<PlanTemplate> _templateCollection;


        public TemplateRepository()
        {
            
            string _connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            
            _templateClient = new MongoClient(_connectionString);
            _templateDatabase = _templateClient.GetDatabase("comwell");
            _templateCollection = _templateDatabase.GetCollection<PlanTemplate>("plantemplate");
        }
        
        public async Task<PlanTemplate> GetPlanTemplate(int id)
        {
            var filter = Builders<PlanTemplate>.Filter.Eq("_id", id);
            return await _templateCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<PlanTemplate>> GetAllPlanTemplates()
        {
            var filter = Builders<PlanTemplate>.Filter.Empty;
            return await _templateCollection.Find(filter).ToListAsync();
        }
    }
}