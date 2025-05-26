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
           var response = await _client.DeleteAsync($"goals/{studentId}/{goal.PlanId}/{goal.ForløbId}/{goal.Id}");

           if (!response.IsSuccessStatusCode)
           {
               throw new Exception("Kunne ikke slette målet");
           }
        }

        public async Task<bool> AddGoal(Goal goal, int studentId)
        {
            var response = await _client.PostAsJsonAsync($"goals/{studentId}/{goal.PlanId}/{goal.ForløbId}", goal);
   
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke tilføje målet");
            }
            return true;
        }

        public async Task<bool> UpdateSkole(Goal goal, int studentId)
        {
            var response = await _client.PutAsJsonAsync($"goals/updateschool/{studentId}", goal);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke opdatere skoleopholdet korrekt");
            }
            return true;
        }


        public async Task<Goal> StartGoal(MentorAssignment mentor)
        {
            var response = await _client.PutAsJsonAsync($"goals/startgoal", mentor);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke starte målet korrekt");
            }
            return await response.Content.ReadFromJsonAsync<Goal>();
        }

        public async Task<Goal> ProcessGoal(MentorAssignment bruger)
        {
            var response = await _client.PutAsJsonAsync($"goals/processgoal", bruger);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke opdatere målet");
            }
            return await response.Content.ReadFromJsonAsync<Goal>();
        }

        public async Task<Goal> ConfirmGoal(int planId, int forløbId, int goalId)
        {
            
            var response = await _client.PutAsJsonAsync($"goals/confirmgoal/{planId}/{forløbId}/{goalId}", new{});

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke bekræfte målet");
            }
            return await response.Content.ReadFromJsonAsync<Goal>();
        }

        public async Task<Goal> ConfirmSchool(int planId, int forløbId, int goalId)
        {
            var response = await _client.PutAsJsonAsync($"goals/confirmschool/{planId}/{forløbId}/{goalId}", new{});

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Kunne ikke bekræfte skoleopholdet");
            }
            return await response.Content.ReadFromJsonAsync<Goal>();
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

        public async Task<List<StartedGoalsDTO>> GetAwaitingApproval(int? hotelId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"goals/awaiting-approval/{hotelId}");
            }
            catch (HttpRequestException)
            {
                return new List<StartedGoalsDTO>();
            }
        }
        
        public async Task<List<KursusManglendeDTO>> GetMissingCourses(int? hotelId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<KursusManglendeDTO>>($"goals/missing-courses/{hotelId}");
            }
            catch (HttpRequestException)
            {
                return new List<KursusManglendeDTO>();
            }
        }   

        public async Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<GoalNeedActionDTO>>($"goals/need-action-goals/{elevId}");
            }
            catch (HttpRequestException)
            {
                return new List<GoalNeedActionDTO>();  
            }
        }

        public async Task<List<FutureSchoolDTO>> GetFutureSchools(int? elevId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<FutureSchoolDTO>>($"goals/future-schools/{elevId}");
            }
            catch (HttpRequestException)
            {
                return new List<FutureSchoolDTO>();
            }
        }

        public async Task<List<OutOfHouseDTO>> GetOutOfHouse(int? hotelId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<OutOfHouseDTO>>($"goals/outofhouse/{hotelId}");
            }
            catch (HttpRequestException)
            {
                return new List<OutOfHouseDTO>();
            }
        }
        
        public async Task<List<StartedGoalsDTO>> GetStartedGoals(int? hotelId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<StartedGoalsDTO>>($"goals/started-goals/{hotelId}");

            }
            catch (HttpRequestException)
            {
                return new List<StartedGoalsDTO>();
            }
            
        }
        
        
    }
}
        