using Client;
using Client.Components.Elevoversigt;
using Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;
using Sprache;

namespace Server
{
    public class GoalRepository : IGoalRepository
    {

        private IMongoClient _goalClient;
        private IMongoDatabase _goalsDatabase;
        private IMongoCollection<User> _goalCollection;
        private readonly IMongoCollection<BsonDocument> _countersCollection;


        public GoalRepository()
        {
            string _connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

            _goalClient = new MongoClient(_connectionString);
            _goalsDatabase = _goalClient.GetDatabase("comwell");
            _goalCollection = _goalsDatabase.GetCollection<User>("users");
            _countersCollection = _goalsDatabase.GetCollection<BsonDocument>("counters");
        }

        //Finder næste fortløbende id
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
        public async Task<bool> DeleteGoal(int studentId, int forløbId, int goalId)
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

        //Tilføjer et nyt goal
        public async Task<bool> AddGoal(int studentId, int forløbId, Goal newGoal)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, studentId),
                Builders<User>.Filter.ElemMatch(u => u.ElevPlan.Forløbs, f => f.Id == forløbId)
            );

            newGoal.Id = await GetNextSequenceValue("goalId");

            var update = Builders<User>.Update.AddToSet("ElevPlan.Forløbs.$.Goals", newGoal);

            var result = await _goalCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        //Tilføjer en kommentar til vores mål
        public async Task<Comment> AddComment(Comment comment)
        {
            Console.WriteLine(
                $"Adding comment to Plan: {comment.PlanId}, Forløb: {comment.ForløbId}, Goal: {comment.GoalId}");

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

            return comment;
        }

        public async Task<bool> UpdateSchoolWithDate(Goal goal, int studentId)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[f].Goals.$[].Status", "InProgress")
                .Set("ElevPlan.Forløbs.$[f].Goals.$[].SkoleNavn", goal.SkoleNavn)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[].StartDate", goal.StartDate)
                .Set("ElevPlan.Forløbs.$[f].Goals.$[].EndDate", goal.EndDate);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f._id", goal.ForløbId)),
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);

            return result.ModifiedCount > 0;

        }

        public async Task<bool> AddStudentToACourse(int studentId, Kursus kursus)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);
            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[].Goals.$[g].Status", "InProgress")
                .Set("ElevPlan.Forløbs.$[].Goals.$[g].StartDate", kursus.StartDate)
                .Set("ElevPlan.Forløbs.$[].Goals.$[g].EndDate", kursus.EndDate);

            var arrayFilter = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("g.CourseCode", kursus.CourseCode))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilter };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);

            return result.ModifiedCount > 0;

        }

        public async Task<bool> CompleteAllStudentsOnCourse(List<int> studentIds, string kursusCode)
        {
            var filter = Builders<User>.Filter.In("_id", studentIds);
            var update = Builders<User>.Update.Set("ElevPlan.Forløbs.$[].Goals.$[g].Status", "Completed");

            var arrayFilter = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("g.CourseCode", kursusCode))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilter };

            var result = await _goalCollection.UpdateManyAsync(filter, update, options);

            return result.ModifiedCount > 0;

        }

        public async Task<bool> RemoveStudentFromACourse(int studentId, string kursusCode)
        {
            var filter = Builders<User>.Filter.Eq("_id", studentId);

            var update = Builders<User>.Update
                .Set("ElevPlan.Forløbs.$[].Goals.$[g].Status", "Active")
                .Unset("ElevPlan.Forløbs.$[].Goals.$[g].StartDate")
                .Unset("ElevPlan.Forløbs.$[].Goals.$[g].EndDate");

            var arrayFilter = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("g.CourseCode", kursusCode))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilter };

            var result = await _goalCollection.UpdateOneAsync(filter, update, options);

            return result.ModifiedCount > 0;

        }

        //Starter en kompetence
        public async Task<Goal> StartGoal(MentorAssignment mentor)
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

            var options = new FindOneAndUpdateOptions<User>
            {
                ArrayFilters = arrayFilters,
                ReturnDocument = ReturnDocument.After

            };

            var result = await _goalCollection.FindOneAndUpdateAsync(filter, update, options);

            var goal = result?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == mentor.ForløbId)?
                .Goals?.FirstOrDefault(g => g.Id == mentor.GoalId);

            return goal;
        }

        public async Task<Goal> ProcessGoal(MentorAssignment mentor)
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
            var goal = user?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == mentor.ForløbId)?
                .Goals?.FirstOrDefault(g => g.Id == mentor.GoalId);

            return goal;
        }

        public async Task<Goal> CompleteGoal(int planId, int forløbId, int goalId)
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

            //return after update option
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
            var confirmedGoal = await CompleteGoal(planId, forløbId, goalId);

            if (confirmedGoal == null)
            {
                return null;

            }

            await UpdateForløbStatus(planId, forløbId);
            return confirmedGoal;
        }


        public async Task<List<Forløb>> GetGoalsByStudentId(int studentId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, studentId);
            var user = await _goalCollection.Find(filter).FirstOrDefaultAsync();

            return user.ElevPlan.Forløbs;
        }

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
            await UpdateYearStatus(planId);

            var goal = updatedUser?.ElevPlan?.Forløbs?
                .FirstOrDefault(f => f.Id == forløbId)?
                .Goals?.FirstOrDefault(g => g.Id == goalId);

            return goal;
        }

        public async Task UpdateYearStatus(int planId)
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
                    .Where(g => g.Type == "Skoleophold")
                    .ToList()
                );

            return await _goalCollection
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetActionGoals(int elevId)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.Eq(u => u.Id, elevId))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Type", "Delmål"),
                    Builders<BsonDocument>.Filter.In("ElevPlan.Forløbs.Goals.Status",
                        new[] { "InProgress", "AwaitingApproval" })))
                .Project(new BsonDocument
                {
                    { },
                });
        }


        //Finder en køkkenchefs manglende godkendelser
        public async Task<List<BsonDocument>> GetAwaitingApproval(int hotelId)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                    Builders<User>.Filter.Eq(u => u.Rolle, "Elev")))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Status", "AwaitingApproval")))
                .Project(new BsonDocument
                {
                    {"FullName", new BsonDocument("$concat", new BsonArray { "$FirstName", " ","$LastName" }) },
                    {"ElevPlan", "$ElevPlan._id"},
                    {"ForløbId", "$ElevPlan.Forløbs.Goals.ForløbId"},
                    {"GoalId", "$ElevPlan.Forløbs.Goals._id"},
                    {"GoalTitle", "$ElevPlan.Forløbs.Goals.Title"}
                });
            
            return await pipeline.ToListAsync();
        }


        public async Task<List<BsonDocument>> GetMissingCourses(int hotelId)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                    Builders<User>.Filter.Eq(u => u.Rolle, "Elev")
                ))
                .AppendStage<BsonDocument>(new BsonDocument("$addFields", new BsonDocument("UserYear", "$Year")))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Type", "Kursus"),
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Status", "Active"),
                    new BsonDocument("$expr", new BsonDocument("$eq", new BsonArray {
                        "$UserYear", "$ElevPlan.Forløbs.Semester"
                    }))
                ))
                .Project(new BsonDocument
                {
                    { "Id", "$_id" },
                    { "FullName", new BsonDocument("$concat", new BsonArray { "$FirstName", " ", "$LastName" }) },
                    { "GoalId", "$ElevPlan.Forløbs.Goals._id" },
                    { "CourseCode", "$ElevPlan.Forløbs.Goals.CourseCode" },
                    { "GoalTitle", "$ElevPlan.Forløbs.Goals.Title" },
                    { "Hotel", "$HotelNavn" }
                });
            return await pipeline.ToListAsync();
        }


        public async Task<List<BsonDocument>> GetOutOfHouse(int hotelId)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                    Builders<User>.Filter.Eq(u => u.Rolle, "Elev")
                ))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.In("ElevPlan.Forløbs.Goals.Type", new[] { "Kursus", "Skoleophold" }),
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Status", "InProgress"),
                    Builders<BsonDocument>.Filter.Ne("ElevPlan.Forløbs.Goals.StartDate", BsonNull.Value),
                    Builders<BsonDocument>.Filter.Ne("ElevPlan.Forløbs.Goals.EndDate", BsonNull.Value),
                    Builders<BsonDocument>.Filter.Lte("ElevPlan.Forløbs.Goals.StartDate", DateTime.Now.AddYears(1))
                ))
                .Sort(Builders<BsonDocument>.Sort.Ascending("ElevPlan.Forløbs.Goals.StartDate"))
                .Project(new BsonDocument
                {
                    { "FullName", new BsonDocument("$concat", new BsonArray { "$FirstName", " ", "$LastName" }) },
                    { "GoalId", "$ElevPlan.Forløbs.Goals._id" },
                    { "GoalTitle", "$ElevPlan.Forløbs.Goals.Title" },
                    { "StartDate", "$ElevPlan.Forløbs.Goals.StartDate" },
                    { "EndDate", "$ElevPlan.Forløbs.Goals.EndDate" }
                });

            return await pipeline.ToListAsync();
        }

        public async Task<List<BsonDocument>> GetStartedGoals(int hotelId)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.HotelId, hotelId),
                    Builders<User>.Filter.Eq(u => u.Rolle, "Elev")
                ))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Type", "Delmål"),
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Status", "InProgress")
                ))
                .Project(new BsonDocument
                {
                    { "FullName", new BsonDocument("$concat", new BsonArray { "$FirstName", " ", "$LastName" }) },
                    { "PlanId", "$ElevPlan.Forløbs.Goals.PlanId" },
                    { "ForløbId", "$ElevPlan.Forløbs.Goals.ForløbId" },
                    { "GoalId", "$ElevPlan.Forløbs.Goals._id" },
                    { "GoalTitle", "$ElevPlan.Forløbs.Goals.Title" }
                });

            return await pipeline.ToListAsync();
        }

        public async Task<List<User>> GetAllStudentsMissingCourse(string kursusCode)
        {
            var pipeline = _goalCollection.Aggregate()
                .Match(Builders<User>.Filter.Eq(u => u.Rolle, "Elev"))
                .Unwind("ElevPlan.Forløbs")
                .Unwind("ElevPlan.Forløbs.Goals")
                .Match(Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Type", "Kursus"),
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.CourseCode", kursusCode),
                    Builders<BsonDocument>.Filter.Eq("ElevPlan.Forløbs.Goals.Status", "Active")
                ))
                .Group(
                    new BsonDocument
                    {
                        { "_id", "$_id" },
                        { "FirstName", new BsonDocument("$first", "$FirstName") },
                        { "LastName", new BsonDocument("$first", "$LastName") },
                        { "HotelNavn", new BsonDocument("$first", "$HotelNavn") }
                    }
                )
                .Project<User>(new BsonDocument
                {
                    { "_id", 1 },
                    { "FirstName", 1 },
                    { "LastName", 1 },
                    { "HotelNavn", 1 }
                });
            return await pipeline.ToListAsync();
        }


    }
}