using System.Net.Http.Json;
using Client.Components.Elevoversigt;
using Core;

namespace Client
{
    public class GoalServiceServer : IGoal
    {

        private string serverUrl = "https://elevportalapi.azurewebsites.net";
        private HttpClient _client;

        public GoalServiceServer(HttpClient client)
        {
            _client = client;
        }

        public async Task DeleteGoal(Goal goal, int studentId)
        {
            await _client.DeleteAsync($"{serverUrl}/goals/{studentId}/{goal.PlanId}/{goal.ForløbId}/{goal.Id}");
        }

        public async Task<bool> AddGoal(Goal goal, int studentId)
        {
            var url = $"{serverUrl}/goals/{studentId}/{goal.PlanId}/{goal.ForløbId}/";
            
            var response = await _client.PostAsJsonAsync(url, goal);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fejl fra server: {errorContent}");
            }
            return response.IsSuccessStatusCode;

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

        public async Task<Goal> ConfirmGoal(int planId, int forløbId, int goalId)
        {
            
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/confirmgoal/{planId}/{forløbId}/{goalId}", new{});

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }

            return null;
        }

        public async Task<Goal> ConfirmSchool(int planId, int forløbId, int goalId)
        {
            var response = await _client.PutAsJsonAsync($"{serverUrl}/goals/confirmschool/{planId}/{forløbId}/{goalId}", new{});

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

        public async Task<List<StartedGoalsDTO>> GetAwaitingApproval(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"{serverUrl}/goals/awaiting-approval/{hotelId}");
        }

        public async Task<List<KursusManglendeDTO>> GetMissingCourses(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<KursusManglendeDTO>>($"{serverUrl}/goals/missing-courses/{hotelId}");
        }

        public async Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId)
        {
            return await _client.GetFromJsonAsync<List<GoalNeedActionDTO>>($"{serverUrl}/goals/need-action-goals/{elevId}");
        }

        public async Task<List<FutureSchoolDTO>> GetFutureSchools(int elevId)
        {
            return await _client.GetFromJsonAsync<List<FutureSchoolDTO>>($"{serverUrl}/goals/future-schools/{elevId}");
        }

        public async Task<List<OutOfHouseDTO>> GetOutOfHouse(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<OutOfHouseDTO>>($"{serverUrl}/goals/outofhouse/{hotelId}");
        }
        
        public async Task<List<StartedGoalsDTO>> GetStartedGoals(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"{serverUrl}/goals/started-goals/{hotelId}");
        }
        
        
    }
}
        