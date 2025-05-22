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

        public async Task<bool> AddGoal(int studentId, int planId, int forløbId, Goal newGoal)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, studentId),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f => f.Id == forløbId)
            );
            
            //Generer id til nyt goal
            newGoal.Id = await GetNextSequenceValue("goalId");

            var update = Builders<User>.Update.AddToSet("ElevPlan.Forløbs.$.Goals", newGoal);

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

        public async Task<Goal> ConfirmGoalHelper(int planId, int forløbId, int goalId)
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
                return null;

            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();
            
            var goal = user?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == forløbId)?
                .Goals?.FirstOrDefault(g => g.Id == goalId);

            return goal;
        }

        public async Task<Goal> ConfirmGoalAndHandleProgress(int planId, int forløbId, int goalId)
        {
            var confirmedGoal = await ConfirmGoalHelper(planId, forløbId, goalId);

            if (confirmedGoal == null)
            {
                return null;
                
            }
            await UpdateForløbStatus(planId, forløbId);
            return confirmedGoal;
        }

        //Skriv om 
        public async Task UpdateForløbStatus(int planId, int forløbId)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", planId);
            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();
    
            var forløb = user?.ElevPlan?.Forløbs?.FirstOrDefault(f => f.Id == forløbId);
    
            if (forløb?.Goals != null && forløb.Goals.All(g => g.Status == "Completed"))
            {
                var updateForløb = Builders<User>.Update
                    .Set("ElevPlan.Forløbs.$[f].Status", "Completed");

                var arrayFilters = new List<ArrayFilterDefinition>
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", forløbId))
                };

                var options = new UpdateOptions { ArrayFilters = arrayFilters };
        
                await _goalCollection.UpdateOneAsync(filter, updateForløb, options);
            }
        }
        
        

        public async Task<Goal> UpdateSchoolStatus(int planId, int forløbId, int goalId)
        {
            var completedAt = DateTime.Now;
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", planId);
            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].Status", "Completed")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[g].CompletedAt", completedAt);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", forløbId)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("g._id", goalId))
            };

            var options = new FindOneAndUpdateOptions<User> 
            { 
                ArrayFilters = arrayFilters,
                ReturnDocument = ReturnDocument.After
            };

            var updatedUser = await _goalCollection.FindOneAndUpdateAsync(filter, update, options);
    
            if (updatedUser == null)
            {
                return null;
            }
            
       
            await UpdateForløbStatus(planId, forløbId);
            await UpdateYearStauts(planId);
    
            var goal = updatedUser?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == forløbId)?
                .Goals?.FirstOrDefault(g => g.Id == goalId);

            return goal;
        }

        public async Task UpdateYearStauts(int planId)
        {
            var filter = Builders<User>.Filter.Eq("ElevPlan._id", planId);
            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();

            if (user.Year != "År 3")
            {
                if (user.Year == "År 1")
                {
                    var update = Builders<User>.Update.Set("Year", "År 2");
                    await _goalCollection.UpdateOneAsync(filter, update);
                }

                if (user.Year == "År 2")
                {
                    var update = Builders<User>.Update.Set("Year", "År 3");
                    await _goalCollection.UpdateOneAsync(filter, update);
                }
            }
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
        

    }
    }