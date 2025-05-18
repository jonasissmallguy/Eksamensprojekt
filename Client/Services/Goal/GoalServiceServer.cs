using System.Net.Http.Json;
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

        public async Task StartGoal(ElevplanComponent.MentorAssignment mentor)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/goals/", mentor);
        }

        public async Task ProcessGoal(ElevplanComponent.MentorAssignment bruger)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/goals/process", bruger);
        }

        public async Task ConfirmGoal(ElevplanComponent.MentorAssignment leder)
        {
            await _client.PutAsJsonAsync($"{serverUrl}/goals/confirm-leder", leder);
        }

        public async Task AddComment(NewComment comment, BrugerLoginDTO currentUser)
        {
            var response = await _client.PostAsJsonAsync($"{serverUrl}/goals/comment", comment);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error adding comment: {response.StatusCode}, {errorContent}");
            }
        }

        public Task DeleteComment(int goalId, int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GoalNameDTO>> GetAllGoalTypes()
        {
            return await _client.GetFromJsonAsync<List<GoalNameDTO>>($"{serverUrl}/goals/types");
        }

        public async Task<List<Goal>> GetAwaitingApproval()
        {
            return await _client.GetFromJsonAsync<List<Goal>>($"{serverUrl}/goals/awaiting-approval");
        }

        public async Task<List<Goal>> GetMissingCourses(User bruger)
        {
            return await _client.GetFromJsonAsync<List<Goal>>($"{serverUrl}/goals/missing-courses/{bruger.Id}");
        }

        public async Task<List<Goal>> GetOutOfHouse()
        {
            return await _client.GetFromJsonAsync<List<Goal>>($"{serverUrl}/goals/out-of-house");
        }

        public async Task ConfirmGoalFromHomePage(Goal goal)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/confirm", goal);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fejl ved opdatering af mål: {error}");
            }
        }

        public async Task<List<Goal>> GetAllGoalsForBruger(User bruger)
        {
            return await _client.GetFromJsonAsync<List<Goal>>($"{serverUrl}/goals/all-for-user/{bruger.Id}");
        }

        public async Task<List<Goal>> GetGoalsByTypeForUser(User bruger, string type)
        {
            var response = await _client.GetFromJsonAsync<List<Goal>>(
                $"{serverUrl}/goals/type/{type}/user/{bruger.Id}"
            );

            return response;
        }


    }
}
    