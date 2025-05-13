using Client.Components.Elevoversigt;
using Core;
using MongoDB.Bson;

namespace Client
{

    public class GoalServiceMock : IGoal
    {

        private List<Goal> _goals;
        
        private IBruger _bruger;
        
        public GoalServiceMock(IBruger bruger)
        {
            _goals = new List<Goal>();
            _bruger = bruger;
        }
        
        public int GenerateId()
        {
            Random rnd = new();
            int id = rnd.Next(1,99999);
            return id;
        }

        public async Task<List<GoalNameDTO>> GetAllGoalTypes()
        {
            var goalTypes = new List<GoalNameDTO>
            {
                new GoalNameDTO() { GoalId = 0, GoalName = "Ingen" },
                new GoalNameDTO() { GoalId = 1, GoalName = "Kursus" },
                new GoalNameDTO() { GoalId = 2, GoalName = "Skole" },
                new GoalNameDTO() { GoalId = 3, GoalName = "Delmål" }
            };
            return goalTypes;
        }
        
        public async Task<Goal> GetGoalByGoalId(int goalId)
        {
            return _goals.FirstOrDefault(x => x.Id == goalId);
        }

        public async Task<Dictionary<int, Goal>> GetAllGoalsByPlanId(int planId)
        {
            var result = _goals.ToDictionary(x => x.Id, x => x);
            return result;
        }

        public async Task<List<Goal>> GetAllUncompletedCourses()
        {
            return _goals.Where(x => x.Type == "Kursus" && x.Status == "Active").ToList();
        }

        public async Task<List<User>> GetUsersByGoalId(int goalId)
        {
            var allStudentIds = _goals.Where(x => x.Id == goalId).ToList().Select(x => x.StudentId).ToList(); //Hmm vi selctor jo egentlig et random, er det ok?
            var allUsers = await _bruger.GetAllUsersByStudentId(allStudentIds);
            
            return allUsers;
            
        }

        public async Task DeleteGoal(Goal goal)
        {
            
            //Opdater vores goal collection
            _goals.RemoveAll(x => x.Id == goal.Id);
            
            //Sletter goal fra forløb collection // FK
            //await _elevPlan.RemoveGoalIdFromForløb(goal);
        }

        public async Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates)
        {
            var newGoals = new List<Goal>();

            foreach (var goal in goalTemplates)
            {
                var nytGoal = new Goal
                {
                    Id = GenerateId(),
                    PlanId = planId,
                    ForløbId = forløbs.Id,
                    Type = goal.Type,
                    Title = goal.Title,
                    Description = goal.Description,
                    Semester = "test",
                    Status = "Active",
                    Comments = new List<Comment>()
                };
                _goals.Add(nytGoal);
                forløbs.GoalIds.Add(nytGoal.Id);
                newGoals.Add(nytGoal);
            }
            
            return newGoals;
        }

        
        
        public async Task StartGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == mentor.GoalId);
            goal.StarterId = mentor.MentorId;
            goal.StarterName = mentor.MentorName;
            goal.StartedAt = DateTime.Now;
            goal.Status = "InProgress";

            goal.Comments.Add(
                new Comment
                {
                    CreatorName = "System",
                    CreatorId = mentor.MentorId,
                    Text = $"Dette er lært af {mentor.MentorName} og sat InProgress"
                });
        }
        
        public async Task ProcessGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == mentor.GoalId);
            goal.ConfirmerId = mentor.MentorId;
            goal.ConfirmerName = mentor.MentorName;
            goal.ConfirmedAt = DateTime.Now;
            goal.Status = "AwaitingApproval";
            
            goal.Comments.Add(
                new Comment
                {
                    CreatorName = "System",
                    CreatorId = mentor.MentorId,
                    Text = $"Dette er lært af {mentor.MentorName} og afventer leder"
                });
            
        }
        
        public async Task ConfirmGoal(ElevplanComponent.MentorAssignment leder)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == leder.GoalId);
            goal.CompletedAt = DateTime.Now;
            goal.Status = "Completed";

            goal.Comments.Add(
                new Comment
                {
                    CreatorName = "System",
                    CreatorId = leder.MentorId,
                    Text = $"Denne opgave er bekræftet af din nærmeste leder"

                });

        }

        public async Task AddComment(NewComment comment, BrugerLoginDTO currentUser)
        {
            var goal =  _goals.FirstOrDefault(x => x.Id == comment.GoalId);

            if (goal != null)
            {
                var nyKomment = new Comment
                {
                    Id = GenerateId(),
                    Text = comment.Comment,
                    CreatorId = currentUser.Id,
                    CreatorName = currentUser.FirstName
                };
                
                goal.Comments.Add(nyKomment);
            }
        }

        public async Task DeleteComment(int goalId, int commentId)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == goalId);
            goal.Comments.Remove(goal.Comments.FirstOrDefault(x => x.Id == commentId));
        }
    }

}