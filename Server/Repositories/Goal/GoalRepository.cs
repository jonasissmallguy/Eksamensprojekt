using Client;
using Client.Components.Elevoversigt;
using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{
    public class GoalRepository : IGoalRepository
    {

        private IMongoClient _goalClient;
        private IMongoDatabase _goalsDatabase;
        private IMongoCollection<User> _goalCollection;
        private IMongoCollection<User> _brugerCollection;
        private readonly IMongoCollection<BsonDocument> _countersCollection;


        public GoalRepository()
        {
            Env.Load();

            string ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            _goalClient = new MongoClient(ConnectionString);
            _goalsDatabase = _goalClient.GetDatabase("comwell");
            _goalCollection = _goalsDatabase.GetCollection<User>("users");
            _brugerCollection = _goalsDatabase.GetCollection<User>("users");
            _countersCollection = _goalsDatabase.GetCollection<BsonDocument>("counters"); 
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
        public async Task<Comment> AddComment(Comment comment)
        {
            Console.WriteLine($"Adding comment to Plan: {comment.PlanId}, Forløb: {comment.ForløbId}, Goal: {comment.GoalId}");

            comment.Id = await GetNextSequenceValue("commentId");
            
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
            
            return comment;
        }

        //Starter en kompetence
        public async Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", mentor.PlanId);

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "InProgress")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].StarterId", mentor.MentorId)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].StarterName", mentor.MentorName)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].StartedAt", DateTime.Now);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", mentor.ForløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", mentor.GoalId))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);
            if (result.ModifiedCount == 0)
                return null;

            //Skal dette laves om - herunder?
            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();
            var goal = user?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == mentor.ForløbId)?
                .Goals?.FirstOrDefault(g => g.Id == mentor.GoalId);

            return goal;
        }
        
        public async Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", mentor.PlanId);

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "AwaitingApproval")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].ConfirmerId", mentor.MentorId)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].ConfirmerName", mentor.MentorName)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].ConfirmedAt", DateTime.Now);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", mentor.ForløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", mentor.GoalId))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);
            if (result.ModifiedCount == 0)
                return null;

            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();
            //Skal dette laves om - herunder?
            var goal = user?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == mentor.ForløbId)?
                .Goals?.FirstOrDefault(g => g.Id == mentor.GoalId);

            return goal;
        }

        public async Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", mentor.PlanId);

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "Completed")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].CompletedAt", DateTime.Now);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", mentor.ForløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", mentor.GoalId))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);
            if (result.ModifiedCount == 0)
                return null;

            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();
            
            //Skal dette laves om - herunder?
            var goal = user?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == mentor.ForløbId)?
                .Goals?.FirstOrDefault(g => g.Id == mentor.GoalId);

            return goal;
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



    }
}