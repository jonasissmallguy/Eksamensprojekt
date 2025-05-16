using Client;
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
        public async Task<IActionResult> PostComment(NewComment comment)
        {
            var commentToAdd = await _goalRepository.AddComment(comment);

            if (commentToAdd)
            {
                return Ok();
            }
            return NotFound();
            
        }
        


        /// <summary>
        /// Tilføjer et nyt delmål til vores forløb
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateGoal(Goal goal)
        {
            return Ok();
        }

    }

}