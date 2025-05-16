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
                new GoalNameDTO() { GoalId = 1, GoalName = "Skole" },
                new GoalNameDTO() { GoalId = 2, GoalName = "Delmål" }
            };
            return goalTypes;
        }
        


        public async Task DeleteGoal(Goal goal, int studentID)
        {
            
            //Sletter goal
            var user = await _bruger.GetBrugerById(studentID);
            var forløb = user.ElevPlan.Forløbs.FirstOrDefault(f => f.Id == goal.ForløbId);
            var deleteGoal = forløb.Goals.FirstOrDefault(x => x.Id == goal.Id);
        }

        
        public async Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløb, List<GoalTemplate> goalTemplates)
        {
            var newGoals = new List<Goal>();

            // Initialize Goals collection if it's null
            if (forløb.Goals == null)
            {
                forløb.Goals = new List<Goal>();
            }

            foreach (var template in goalTemplates)
            {
                var newGoal = new Goal
                {
                    Id = GenerateId(),
                    Type = template.Type,
                    Title = template.Title,
                    Description = template.Description,
                    Semester = forløb.Semester, 
                    Status = "Active",
                    Comments = new List<Comment>()
                };
        
                _goals.Add(newGoal);        
                forløb.Goals.Add(newGoal);  
                newGoals.Add(newGoal);      
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


        }
        
        public async Task ProcessGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == mentor.GoalId);
            goal.ConfirmerId = mentor.MentorId;
            goal.ConfirmerName = mentor.MentorName;
            goal.ConfirmedAt = DateTime.Now;
            goal.Status = "AwaitingApproval";
            

        }
        
        public async Task ConfirmGoal(ElevplanComponent.MentorAssignment leder)
        {
            var goal = _goals.FirstOrDefault(x => x.Id == leder.GoalId);
            goal.CompletedAt = DateTime.Now;
            goal.Status = "Completed";
            

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