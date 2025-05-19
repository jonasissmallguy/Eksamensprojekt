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
                    PlanId = comment.PlanId,
                    ForløbId = comment.ForløbId,
                    GoalId = comment.GoalId,
                    CreatorId = comment.CommentorId,
                    CreatorName = comment.CommentName,
                    Text = comment.Comment,
                    CreatedAt = DateTime.Now
                };

                var commentToAdd = await _goalRepository.AddComment(newComment);

                if (commentToAdd == null)
                {
                    return BadRequest("Fejl - kunne ikke tilføje kommentar");
                }
                
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

        /// <summary>
        /// Starter goal
        /// </summary>
        /// <param name="bruger"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("startgoal")]
        public async Task<IActionResult> StartGoal(ElevplanComponent.MentorAssignment bruger)
        {
            var goal = await _goalRepository.StartGoal(bruger);

            if (goal == null)
            {
                return BadRequest();
            }
            return Ok(goal);
        }

        [HttpPut]
        [Route("processgoal")]
        public async Task<IActionResult> ProcessGoal(ElevplanComponent.MentorAssignment bruger)
        {
            var processedGoal = await _goalRepository.ProcessGoal(bruger);

            if (processedGoal == null)
            {
                return BadRequest();
            }
            
            return Ok(processedGoal);
        }
        
        [HttpPut]
        [Route("confirmgoal")]
        public async Task<IActionResult> ConfirmGoal(ElevplanComponent.MentorAssignment bruger)
        {
            var processedGoal = await _goalRepository.ConfirmGoal(bruger);

            if (processedGoal == null)
            {
                return BadRequest();
            }
            
            return Ok(processedGoal);
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

        
    }

}