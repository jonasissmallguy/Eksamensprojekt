using Core;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    
    [ApiController]
    [Route("goal")]
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
        [Route("{planId}/{studentId}/{goalId}")]
        public async Task<IActionResult> DeleteGoal(int planId, int studentId, int goalId)
        {
            var delete =  await _goalRepository.DeleteGoal(planId, studentId, goalId);
            return Ok();
        }


        /// <summary>
        /// Tilføjer en kommentar til vores goal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateGoal(Goal goal)
        {
            return Ok();
        }

    }

}