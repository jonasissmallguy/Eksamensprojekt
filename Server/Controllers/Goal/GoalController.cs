using Client;
using Client.Components.Elevoversigt;
using Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;

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

        [Route("need-action-goals/{elevId}")]
        public async Task<IActionResult> NeedActionGoals(int elevId)
        {
            var users = await _goalRepository.GetActionGoals(elevId);

            var result = new List<GoalNeedActionDTO>();

            foreach (var user in users)
            {
                var name = user.FirstName + " " + user.LastName;

                var actionGoals = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Status == "InProgress" || g.Status == "AwaitingApproval")
                    .ToList();

                foreach (var goal in actionGoals)
                {
                    var status = goal.Status 
                        switch
                    {
                        "InProgress" => "Mangler godkendelse fra en kok eller din leder",
                        "AwaitingApproval" => "Afventer din leders godkendelse"
                    };

                    result.Add(new GoalNeedActionDTO
                    {
                        GoalId = goal.Id,
                        GoalTitle = goal.Title,
                        Status = status
                    });
                }
            }
            return Ok(result);
        }

        
        /// <summary>
        /// Henter mål for en køkkenchef som mangler godkendelse 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("awaiting-approval/{hotelId}")]
        public async Task<IActionResult> GetAwaitingApproval(int hotelId)
        {
            var users = await _goalRepository.GetAwaitingApproval(hotelId);

            var result = new List<StartedGoalsDTO>();

            foreach (var user in users)
            {
                var name = user.FirstName + " " + user.LastName;
                
                var awaitingApprovalGoals = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Status == "AwaitingApproval")
                    .ToList();
                

                if (awaitingApprovalGoals != null)
                {
                    foreach (var goal in awaitingApprovalGoals)
                    {
                        result.Add(new StartedGoalsDTO
                        {
                            FullName = name,
                            PlanId = goal.PlanId,
                            ForløbId = goal.ForløbId,
                            GoalId = goal.Id,
                            GoalTitle = goal.Title
                        });
                    }
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Indeholder mål der er InProgress
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("started-goals/{hotelId}")]
        public async Task<IActionResult> GetStartedGoals(int hotelId)
        {
            var users = await _goalRepository.GetStartedGoals(hotelId);
            
            var result = new List<StartedGoalsDTO>();

            foreach (var user in users)
            {
                var name = user.FirstName + " " + user.LastName;
                
                var inProgressGoals = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Status == "InProgress")
                    .ToList();

                if (inProgressGoals != null)
                {
                    foreach (var goal in inProgressGoals)
                    {
                        result.Add(new StartedGoalsDTO
                        {
                            FullName = name,
                            PlanId = goal.PlanId,
                            ForløbId = goal.ForløbId, 
                            GoalId = goal.Id,
                            GoalTitle = goal.Title
                        });
                    }
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Henter alle kurser en elev mangler 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [HttpGet("missing-courses/{hotelId}")]
        public async Task<IActionResult> GetMissingCourses(int hotelId)
        {
            var users = await _goalRepository.GetMissingCourses(hotelId);

            var result = new List<KursusManglendeDTO>();

            foreach (var user in users)
            {
                var name = user.FirstName + " " + user.LastName;

                var missingCourses = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Type == "Kursus")
                    .ToList();

                if (missingCourses != null)
                {
                    foreach (var kursus in missingCourses)
                    {
                        result.Add(new KursusManglendeDTO
                        {
                            FullName = name,
                            GoalId = kursus.Id,
                            GoalTitle = kursus.Title
                        });
                    }
                }

            }
            
            return Ok(result);
        }

        [HttpGet]
        [Route("outofhouse/{hotelId}")]
        public async Task<IActionResult> GetOutOfHouse(int hotelId)
        {
            var users = await _goalRepository.GetOutOfHouse(hotelId);
            
            var result = new List<OutOfHouseDTO>();

            foreach (var user in users)
            {
                var name = user.FirstName + " " + user.LastName;

                var outOfHouseGoals = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Type == "Kursus" || g.Type == "Skoleforløb")
                    .ToList();

                if (outOfHouseGoals != null)
                {
                    foreach (var goal in outOfHouseGoals)
                    {
                        result.Add(new OutOfHouseDTO
                        {
                            FullName = name,
                            GoalId = goal.Id,
                            GoalTitle = goal.Title,
                            StartDate = goal.StartDate,
                            EndDate = goal.EndDate
                        });
                    }
                }

            }
            
            return Ok(result);
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
        public async Task<IActionResult> ConfirmGoalFromHomePage([FromBody] StartedGoalsDTO goalDto)
        {
            if (goalDto == null || goalDto.GoalId <= 0)
                return BadRequest("Ugyldigt mål data.");
            
            var updated = await _goalRepository.ConfirmGoalFromHomePage(goalDto.PlanId, goalDto.ForløbId, goalDto.GoalId);
            if (updated)
                return Ok();
            return NotFound("Mål ikke fundet til opdatering.");
        }


        //Hvad bruges den her til?
        [HttpGet("type/{type}/user/{userId}")]
        public async Task<IActionResult> GetGoalsByTypeForUser(string type, int userId)
        {
            var goals = await _goalRepository.GetGoalsByTypeForUser(type, userId);

            if (goals == null || !goals.Any())
                return NotFound("Ingen mål fundet for bruger og type.");

            return Ok(goals);
        }

        //Denne skal slettes
        [HttpGet("types")]
        public async Task<IActionResult> GetAllGoalTypes()
        {
            var types = await _goalRepository.GetAllGoalTypes();
            return Ok(types);
        }
        
        //Hvad bruges den her til?
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