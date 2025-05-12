using Client.Components.Elevoversigt;
using Core;
using MongoDB.Bson;

namespace Client
{

    public class GoalServiceMock : IGoal
    {

        private List<Goal> _goals;
        
        public GoalServiceMock()
        {
            _goals = new List<Goal>();
        }
        
        public int GenerateId()
        {
            Random rnd = new();
            int id = rnd.Next(1,99999);
            return id;
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

        public async Task DeleteGoal(Goal goal)
        {
            
            //Opdater vores goal collection
            _goals.RemoveAll(x => x.Id == goal.Id);
            
            //Sletter goal fra forløb collection
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

        public async Task ConfirmGoal(int goalId)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == goalId);
            goal.Status = "Completed";
        }

        public async Task AddMentorToGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == mentor.GoalId);
            
            goal.MentorId = mentor.MentorId;
            Console.WriteLine("denne opgave er nu assigned til" + goal.MentorId);
        }

        public async Task RemoveMentorFromGoal(int goalId)
        {
            var goalRemove = _goals.FirstOrDefault(x => x.Id == goalId);
            
            goalRemove.MentorId = null;
            goalRemove.MentorName = string.Empty;
            
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

        public Task DeleteComment(int goalId, int commentId)
        {
            throw new NotImplementedException();
        }
    }

}