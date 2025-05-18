using Client;
using Client.Components.Elevoversigt;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    
    [ApiController]
    [Route("goals")]
    public class GoalController : ControllerBase
    {

        private IGoalRepository _goalRepository;

        public GoalController(IGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }
        
        /// <summary>
        /// Sletter et mål i den nested struktur
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="studentId"></param>
        /// <param name="goalId"></param>
        [HttpDelete]
        [Route("{studentId}/{planId}/{forløbId}/{goalId}")]
        public async Task<IActionResult> DeleteGoal(int studentId, int planId, int forløbId, int goalId)
        {
            var delete =  await _goalRepository.DeleteGoal(studentId, planId, forløbId, goalId);

            if (delete)
            {
                return Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Tilføjer en kommentar til vores goal
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comment")]
        public async Task<IActionResult> PostComment(NewComment comment)
        {
            try
            {
                if (comment == null)
                    return BadRequest("Data");

                var newComment = new Comment
                {
                    CreatorId = comment.CommentorId,
                    CreatorName = comment.CommentName,
                    Text = comment.Comment
                };

                var commentToAdd = await _goalRepository.AddComment(newComment);

                return Ok(commentToAdd);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        
        [HttpGet("awaiting-approval")]
        public async Task<IActionResult> GetAwaitingApproval()
        {
            var goals = await _goalRepository.GetAwaitingApproval();
            return Ok(goals);
        }

        [HttpGet("missing-courses/{userId}")]
        public async Task<IActionResult> GetMissingCourses(int userId)
        {
            var goals = await _goalRepository.GetMissingCourses(userId);
            return Ok(goals);
        }

        [HttpGet("out-of-house")]
        public async Task<IActionResult> GetOutOfHouse()
        {
            var goals = await _goalRepository.GetOutOfHouse();
            return Ok(goals);
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmGoalFromHomePage([FromBody] Goal goal)
        {
            if (goal == null || goal.Id <= 0)
                return BadRequest("Ugyldigt mål data.");

            var updated = await _goalRepository.ConfirmGoalFromHomePage(goal);
            if (updated)
                return Ok();
            return NotFound("Mål ikke fundet til opdatering.");
        }

        
        [HttpGet("type/{type}/user/{userId}")]
        public async Task<IActionResult> GetGoalsByTypeForUser(string type, int userId)
        {
            var goals = await _goalRepository.GetGoalsByTypeForUser(type, userId);

            if (goals == null || !goals.Any())
                return NotFound("Ingen mål fundet for bruger og type.");

            return Ok(goals);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetAllGoalTypes()
        {
            var types = await _goalRepository.GetAllGoalTypes();
            return Ok(types);
        }

        [HttpGet("all-for-user/{userId}")]
        public async Task<IActionResult> GetAllGoalsForUser(int userId)
        {
            var goals = await _goalRepository.GetAllGoalsForUser(userId);
            if (goals == null || !goals.Any())
                return NotFound("Ingen mål fundet for brugeren.");
            return Ok(goals);
        }
    }

}