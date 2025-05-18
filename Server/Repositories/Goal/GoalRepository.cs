using Client;
using Client.Components.Elevoversigt;
using Core;
using MongoDB.Driver;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Server
{
    public class GoalRepository : IGoalRepository
    {

        private IMongoClient _goalClient;
        private IMongoDatabase _goalsDatabase;
        private IMongoCollection<User> _goalCollection;
        private IMongoCollection<User> _brugerCollection;

        public GoalRepository()
        {
            Env.Load();

            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            _goalClient = new MongoClient(ConnectionString);
            _goalsDatabase = _goalClient.GetDatabase("comwell");
            _goalCollection = _goalsDatabase.GetCollection<User>("users");
            _brugerCollection = _goalsDatabase.GetCollection<User>("users");

        }


        
        //Fjerner et goal i vores nestedarray 
        public async Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, studentId),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f => f.Id == forløbId)
            );

            var update = Builders<User>.Update.PullFilter("ElevPlan.Forløbs.$.Goals",
                Builders<Goal>.Filter.Eq(g => g.Id, goalId));

            var result = await _goalCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        //Tilføjer en kommentar til vores mål
        public async Task<bool> AddComment(Comment comment)
        {
            Console.WriteLine($"Adding comment to Plan: {comment.PlanId}, Forløb: {comment.ForløbId}, Goal: {comment.GoalId}");
    
            // Use the exact field names from your document structure
            // Notice the document uses _id fields, not Id fields
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", comment.PlanId);
    
            var update = Builders<User>.Update.Push("ElevPlan.Forløbs.$[f].Goals.$[g].Comments", comment);
    
            var arrayFilters = new List<ArrayFilterDefinition>
            {
                // Notice the use of _id instead of Id based on your document structure
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", comment.ForløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", comment.GoalId))
            };
    
            var options = new UpdateOptions { ArrayFilters = arrayFilters };
    
            var result = await _goalCollection.UpdateOneAsync(filter, update, options);
            Console.WriteLine($"Result: ModifiedCount={result.ModifiedCount}, MatchedCount={result.MatchedCount}");
    
            return result.ModifiedCount > 0;
        }

        public Task StartGoal(ElevplanComponent mentor)
        {
            throw new NotImplementedException();
        }
        
        public async Task<List<Goal>> GetAwaitingApproval()
        {
        var filter = Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f =>
        f.Goals.Any(g => g.Status == "AwaitingApproval"));

        var users = await _goalCollection.Find(filter).ToListAsync();
        var goals = new List<Goal>();

        foreach (var user in users)
        {
        foreach (var forløb in user.ElevPlan.Forløbs)
        {
            goals.AddRange(forløb.Goals.Where(g => g.Status == "AwaitingApproval"));
        }
        }

        return goals;
        }

        public async Task<List<Goal>> GetMissingCourses(int userId)
        {
        var user = await _goalCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
        var missing = new List<Goal>();

        if (user != null)
        {
        foreach (var forløb in user.ElevPlan.Forløbs)
        {
            missing.AddRange(forløb.Goals.Where(g => g.Type == "Kursus" && g.Status != "Finished"));
        } 
        }

        return missing;
        }

        public async Task<List<Goal>> GetOutOfHouse()
        {
        var filter = Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f =>
        f.Goals.Any(g => g.Type == "Skole"));

        var users = await _goalCollection.Find(filter).ToListAsync();
        var result = new List<Goal>();

        foreach (var user in users)
        {
        foreach (var forløb in user.ElevPlan.Forløbs)
        {
            result.AddRange(forløb.Goals.Where(g => g.Type == "Skole"));
        }
        }

        return result;
        }

        public async Task<bool> ConfirmGoalFromHomePage(Goal goal)
        {
            if (goal == null || goal.Id <= 0)
                return false;

            // Find user som indeholder et forløb med målet
            var filter = Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f =>
                f.Goals.Any(g => g.Id == goal.Id));

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "Finished")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].ConfirmedAt", DateTime.Now);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Goals._id", goal.Id)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", goal.Id))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);

            if (result.ModifiedCount == 0)
                Console.WriteLine($"ConfirmGoalFromHomePage: Kunne ikke opdatere mål med Id={goal.Id}");

            return result.ModifiedCount > 0;
        }



        
        public async Task<List<Goal>> GetGoalsByTypeForUser(string type, int userId)
        {
            var bruger = await _brugerCollection.Find(b => b.Id == userId).FirstOrDefaultAsync();
            if (bruger == null || bruger.ElevPlan == null || bruger.ElevPlan.Forløbs == null)
                return new List<Goal>();

            var result = new List<Goal>();

            foreach (var forløb in bruger.ElevPlan.Forløbs)
            {
                result.AddRange(forløb.Goals.Where(g => g.Type == type && g.Status == "Active"));
            }

            return result;
        }

        public async Task<List<string>> GetAllGoalTypes()
        {
            // Antag at alle måltyper kan findes ved at aggregere over alle goals og hente unikke typer
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$unwind", "$ElevPlan.Forløbs"),
                new BsonDocument("$unwind", "$ElevPlan.Forløbs.Goals"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$ElevPlan.Forløbs.Goals.Type" }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Type", "$_id" },
                    { "_id", 0 }
                })
            };

            var result = await _goalCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return result.Select(d => d["Type"].AsString).ToList();
        }

        public async Task<List<Goal>> GetAllGoalsForUser(int userId)
        {
            var user = await _goalCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user?.ElevPlan?.Forløbs == null)
                return new List<Goal>();

            var allGoals = new List<Goal>();
            foreach (var forløb in user.ElevPlan.Forløbs)
            {
                allGoals.AddRange(forløb.Goals);
            }

            return allGoals;
        }

    }
}