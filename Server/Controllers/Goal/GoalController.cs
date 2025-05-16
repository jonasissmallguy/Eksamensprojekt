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
        
    }

}