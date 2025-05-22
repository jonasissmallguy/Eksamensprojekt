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
            string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            
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

        public async Task<List<Goal>> GetFutureSchools(int elevId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, elevId);

            var projection = Builders<User>.Projection
                .Expression(u => u.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Type == "Skoleforløb")
                    .ToList()
                );

            return await _goalCollection
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetActionGoals(int elevId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, elevId),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.And(
                            Builders<Goal>.Filter.Eq(g => g.Type, "Delmål"),
                            Builders<Goal>.Filter.In(g => g.Status, new[] { "InProgress", "AwaitingApproval" })
                        )
                    )
                )
            );
            
            return await _goalCollection.Find(filter).ToListAsync();
        }


        //Finder en køkkenchefs manglende godkendelser
        public async Task<List<User>> GetAwaitingApproval(int hotelId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                Builders<User>.Filter.Eq(u => u.Rolle, "Elev"),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.Eq(g => g.Status, "AwaitingApproval")
                    )
                )
            );
            return await _goalCollection.Find(filter).ToListAsync();
        }


        public async Task<List<User>> GetMissingCourses(int hotelId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                Builders<User>.Filter.Eq(u => u.Rolle, "Elev"),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.Eq(g => g.Type, "Kursus")
                    )
                )
            );

            var projection = Builders<User>.Projection
                .Include(u => u.FirstName)
                .Include(u => u.LastName)
                .Include(u => u.ElevPlan.Forløbs); 

            return await _goalCollection
                .Find(filter)
                .Project<User>(projection)
                .ToListAsync();
        }
        
        
        public async Task<List<User>> GetOutOfHouse(int hotelId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                Builders<User>.Filter.Eq(u => u.Rolle, "Elev"),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.And(
                            Builders<Goal>.Filter.In(g => g.Type, new[] { "Kursus", "Skoleforløb" }),
                            Builders<Goal>.Filter.Eq(g => g.Status, "Active")
                        )
                    )
                )
            );

            return await _goalCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> ConfirmGoalFromHomePage(int planId, int forløbId, int goalId)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", planId);

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "Completed")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].CompletedAt", DateTime.Now);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", forløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", goalId))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);
            
            if (result.ModifiedCount == 0)
                return false;

            return true;
        }

        public async Task<List<User>> GetStartedGoals(int hotelId)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                Builders<User>.Filter.Eq(u => u.Rolle, "Elev"),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs,
                    Builders<Forløb>.Filter.ElemMatch(f => f.Goals,
                        Builders<Goal>.Filter.And(
                            Builders<Goal>.Filter.Eq(g => g.Type, "Delmål"),
                            Builders<Goal>.Filter.Eq(g => g.Status, "InProgress") 
                        )
                    )
                )
            );
            
            return await _goalCollection.Find(filter).ToListAsync();
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

        //Denne skal slettes
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

        //Slet??
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