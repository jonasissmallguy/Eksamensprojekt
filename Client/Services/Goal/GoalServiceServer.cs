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

        public async Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/startgoal", mentor);

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }

            return null;
        }

        public async Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment bruger)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/processgoal", bruger);

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }
            return null;
            
        }

        public async Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment leder)
        {
            Console.WriteLine(leder.MentorName);
            
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/confirmgoal", leder);

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }

            return null;
        }

        public async Task<Comment> AddComment(NewComment comment)
        {
            var response = await _client.PostAsJsonAsync($"{serverUrl}/goals/comment", comment);
            if (response.IsSuccessStatusCode)
            {
                var addedComment = await response.Content.ReadFromJsonAsync<Comment>();
                return addedComment;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error adding comment: {response.StatusCode}, {errorContent}");
                return null;
            }
        }
        public Task DeleteComment(int goalId, int commentId)
        {
            throw new NotImplementedException();
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

        public async Task<List<AwaitingApprovalDTO>> GetAwaitingApproval(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<AwaitingApprovalDTO>>($"{serverUrl}/goals/awaiting-approval/{hotelId}");
        }

        public async Task<List<KursusManglendeDTO>> GetMissingCourses(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<KursusManglendeDTO>>($"{serverUrl}/goals/missing-courses/{hotelId}");
        }

        public async Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId)
        {
            return await _client.GetFromJsonAsync<List<GoalNeedActionDTO>>($"{serverUrl}/goals/need-action-goals/{elevId}");
        }

        public async Task<List<OutOfHouseDTO>> GetOutOfHouse(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<OutOfHouseDTO>>($"{serverUrl}/goals/outofhouse/{hotelId}");
        }

        public async Task<bool> ConfirmGoalFromHomePage(AwaitingApprovalDTO goal)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/confirm", goal);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fejl ved opdatering af mål: {error}");
            }

            return true;
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
    