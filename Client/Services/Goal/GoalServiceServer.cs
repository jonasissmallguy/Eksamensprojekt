using Client.Components.Elevoversigt;
using Core;

namespace Client
{
    public class GoalServiceServer : IGoal
    {
        
        private string serverUrl = "http://localhost:5075";
        private HttpClient _client;

        public GoalServiceServer(HttpClient client)
        {
            _client = client;
        }
        
        public async Task DeleteGoal(Goal goal, int studentId)
        {
            await _client.DeleteAsync($"{serverUrl}/goals/{studentId}/{goal.PlanId}/{goal.ForløbId}/{goal.Id}");
        }

        public Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates)
        {
            throw new NotImplementedException();
        }

        public Task StartGoal(ElevplanComponent.MentorAssignment mentor)
        {
            throw new NotImplementedException();
        }

        public Task ProcessGoal(ElevplanComponent.MentorAssignment bruger)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmGoal(ElevplanComponent.MentorAssignment leder)
        {
            throw new NotImplementedException();
        }

        public Task AddComment(NewComment comment, BrugerLoginDTO currentUser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteComment(int goalId, int commentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<GoalNameDTO>> GetAllGoalTypes()
        {
            throw new NotImplementedException();
        }
    }
}