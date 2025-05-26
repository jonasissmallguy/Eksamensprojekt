using System.Net.Http.Json;
using Client.Components.Elevoversigt;
using Core;

namespace Client
{
    public class GoalServiceServer : IGoal
    {
        
        private HttpClient _client;

        public GoalServiceServer(HttpClient client)
        {
            _client = client;
        }

        public async Task DeleteGoal(Goal goal, int studentId)
        {
            await _client.DeleteAsync($"goals/{studentId}/{goal.PlanId}/{goal.ForløbId}/{goal.Id}");
        }

        public async Task<bool> AddGoal(Goal goal, int studentId)
        {
            var response = await _client.PostAsJsonAsync($"goals/{studentId}/{goal.PlanId}/{goal.ForløbId}", goal);
   
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fejl fra server: {errorContent}");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateSkole(Goal goal, int studentId)
        {
            var response = await _client.PutAsJsonAsync($"goals/updateschool/{studentId}", goal);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
            }
            return response.IsSuccessStatusCode;
        }


        public async Task<Goal> StartGoal(MentorAssignment mentor)
        {
            var response = await _client.PutAsJsonAsync($"goals/startgoal", mentor);

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }

            return null;
        }

        public async Task<Goal> ProcessGoal(MentorAssignment bruger)
        {
            var response = await _client.PutAsJsonAsync($"goals/processgoal", bruger);

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }
            return null;
            
        }

        public async Task<Goal> ConfirmGoal(int planId, int forløbId, int goalId)
        {
            
            var response = await _client.PutAsJsonAsync($"goals/confirmgoal/{planId}/{forløbId}/{goalId}", new{});

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }

            return null;
        }

        public async Task<Goal> ConfirmSchool(int planId, int forløbId, int goalId)
        {
            var response = await _client.PutAsJsonAsync($"goals/confirmschool/{planId}/{forløbId}/{goalId}", new{});

            if (response.IsSuccessStatusCode)
            {
                var goal = await response.Content.ReadFromJsonAsync<Goal>();
                return goal;
            }
            return null;
        }

        public async Task<List<GoalProgessDTO>> GoalProgess(int studentId)
        {
            return await _client.GetFromJsonAsync<List<GoalProgessDTO>>($"goals/progress/{studentId}");
        }

        public async Task<Comment> AddComment(NewComment comment)
        {
            var response = await _client.PostAsJsonAsync($"goals/comment", comment);
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
            return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"goals/awaiting-approval/{hotelId}");
        }

        public async Task<List<KursusManglendeDTO>> GetMissingCourses(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<KursusManglendeDTO>>($"goals/missing-courses/{hotelId}");
        }

        public async Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId)
        {
            return await _client.GetFromJsonAsync<List<GoalNeedActionDTO>>($"goals/need-action-goals/{elevId}");
        }

        public async Task<List<FutureSchoolDTO>> GetFutureSchools(int elevId)
        {
            return await _client.GetFromJsonAsync<List<FutureSchoolDTO>>($"goals/future-schools/{elevId}");
        }

        public async Task<List<OutOfHouseDTO>> GetOutOfHouse(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<OutOfHouseDTO>>($"goals/outofhouse/{hotelId}");
        }
        
        public async Task<List<StartedGoalsDTO>> GetStartedGoals(int hotelId)
        {
            return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"goals/started-goals/{hotelId}");
        }
        
        
    }
}
        