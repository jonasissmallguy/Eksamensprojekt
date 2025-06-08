using Client;
using Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        /// Sletter et mål under elevplanen
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="planId"></param>
        /// <param name="forløbId"></param>
        /// <param name="goalId"></param>
        /// <returns>Status 200</returns>
        [HttpDelete]
        [Route("{studentId}/{planId}/{forløbId}/{goalId}")]
        public async Task<IActionResult> DeleteGoal(int studentId, int planId, int forløbId, int goalId)
        {
            if (studentId <= 0 || planId <= 0 || forløbId <= 0 || goalId <= 0)
            {
                return BadRequest("Forkert studentId, planId, forløbId eller goalId");
            }
            
            var delete =  await _goalRepository.DeleteGoal(studentId, forløbId, goalId);

            if (!delete)
            {
                return NotFound("Kunne ikke finde målet");
            }
            return Ok();
        }
        
        /// <summary>
        /// Tilføjer et mål til en elevs elevplan
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="planId"></param>
        /// <param name="forløbId"></param>
        /// <param name="newGoal"></param>
        /// <returns>Status 200</returns>
        [HttpPost]
        [Route("{studentId}/{planId}/{forløbId}")]
        public async Task<IActionResult> AddGoal(int studentId, int planId, int forløbId, [FromBody] Goal newGoal)
        {
            if (newGoal == null || studentId <= 0 || newGoal.PlanId <= 0)
            {
                return BadRequest("Mangler påkrævede felter");
            }

            if (newGoal.ForløbId <= 0)
            {
                return BadRequest("Venligst vælg et forløb");
            }
            
            //Validering af tilføjelse af nyt delmål
            if (newGoal.Type == "Delmål")
            {
                if (string.IsNullOrWhiteSpace(newGoal.Title))
                {
                    return BadRequest("Venligst indtast en titel");
                }

                if (string.IsNullOrWhiteSpace(newGoal.Description))
                {
                    return BadRequest("Venligst indtast en beskrivelse");
                }
            }
            
            var add = await _goalRepository.AddGoal(studentId, forløbId, newGoal);

            if (!add)
            {
                return BadRequest("Kunne ikke tilføje goalet  ");
            }
            return Ok();
        }

        /// <summary>
        /// Opdater et skolegoal
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="studentId"></param>
        /// <returns>Status 200</returns>
        [HttpPut]
        [Route("updateschool/{studentId}")]
        public async Task<IActionResult> UpdateSkole([FromBody] Goal goal, int studentId)
        {
            if (goal == null || studentId <= 0)
            {
                return BadRequest("Input er forkert");
            }

            if (!goal.StartDate.HasValue || !goal.EndDate.HasValue)
            {
                return BadRequest("Start- og slutdato skal udfyldes.");
            }

            if (goal.StartDate > goal.EndDate)
            {
                return BadRequest("Mismatch i start og slutdato");
            }

            if (string.IsNullOrWhiteSpace(goal.SkoleNavn))
            {
                return BadRequest("Skolenavn skal være udfyldt");
            }
            
            
            var updateResult = await _goalRepository.UpdateSchoolWithDate(goal, studentId);

            if (!updateResult)
            {
                return NotFound("Kunne ikke opdatere skolegoalet");
            }

            return Ok();
        }


        /// <summary>
        /// Tilføjer en kommentar til vores goal
        /// </summary>
        /// <param name="goal"></param>
        /// <returns>Kommentaren, som vi har tilføjet</returns>
        [HttpPost]
        [Route("comment")]
        public async Task<IActionResult> PostComment(NewComment comment)
        {
            try
            {
                if (comment == null)
                    return BadRequest("Data mangler");

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
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Finder alle mål for en elev med type = delmål , der har statusen InProgress eller AwaitingApproval
        /// </summary>
        /// <param name="elevId"></param>
        /// <returns>Liste med GoalNeedActionDTO</returns> 
        [HttpGet]
        [Route("need-action-goals/{elevId}")]
        public async Task<IActionResult> NeedActionGoals(int elevId)
        {
            if (elevId <= 0)
            {
                return BadRequest("Forkert elevId");
            }
            
            var users = await _goalRepository.GetActionGoals(elevId);

            var result = new List<GoalNeedActionDTO>();

            foreach (var user in users)
            {
                var actionGoals = user.ElevPlan.Forløbs
                    .SelectMany(f => f.Goals)
                    .Where(g => g.Type == "Delmål" && g.Status == "InProgress" || g.Status == "AwaitingApproval")
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
        /// <returns>Retunerer en liste af StartedGoalsDTO</returns>
        [HttpGet]
        [Route("awaiting-approval/{hotelId}")]
        public async Task<IActionResult> GetAwaitingApproval(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest("Forkert hotelId");
            }
            
            var docs = await _goalRepository.GetAwaitingApproval(hotelId);
            
            var result = docs.Select(x => new StartedGoalsDTO
            {
                FullName = x["FullName"].ToString(),
                PlanId = x["ElevPlan"].ToInt32(),
                ForløbId = x["ForløbId"].ToInt32(),
                GoalId = x["GoalId"].ToInt32(),
                GoalTitle = x["GoalTitle"].ToString(),
            }).ToList();
            
            return Ok(result);
        }

        /// <summary>
        /// Indeholder Goal med typen = Delmål der er InProgress
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns>Retunerer en liste af StartedGoalsDTO</returns>
        [HttpGet]
        [Route("started-goals/{hotelId}")]
        public async Task<IActionResult> GetStartedGoals(int hotelId)
        {
            if (hotelId <= 0)
                return BadRequest("Invalid hotelId");

            var docs = await _goalRepository.GetStartedGoals(hotelId);

            var result = docs.Select(d => new StartedGoalsDTO
            {
                FullName = d["FullName"].AsString,
                PlanId = d["PlanId"].AsInt32,
                ForløbId = d["ForløbId"].AsInt32,
                GoalId = d["GoalId"].AsInt32,
                GoalTitle = d["GoalTitle"].AsString
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Henter alle kurser en elev mangler med status = Active
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns>En liste af KursusManglendeDTO</returns>
        [HttpGet("missing-courses/{hotelId}")]
        public async Task<IActionResult> GetMissingCourses(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest("Forkert hotelId");
            }
            
            var docs = await _goalRepository.GetMissingCourses(hotelId);
            

            var result = docs.Select(x => new KursusManglendeDTO
            {
                Id = x["Id"].AsInt32,
                GoalTitle = x["GoalTitle"].AsString,
                CourseCode = x["CourseCode"].AsString,
                FullName = x["FullName"].AsString,
                GoalId = x["GoalId"].AsInt32,
                Hotel = x["Hotel"].AsString,
            });
            
            return Ok(result);
        }

        /// <summary>
        /// Finder alle goals med typen kursus eller skoleophold, med status = InProgress for dd. og 1 år frem
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns>En liste af OutOfHouseDTO</returns>
        [HttpGet]
        [Route("outofhouse/{hotelId}")]
        public async Task<IActionResult> GetOutOfHouse(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest("Forkert hotelId");
            }
            
            var docs = await _goalRepository.GetOutOfHouse(hotelId);
            
            var result = docs.Select(x => new OutOfHouseDTO
            {
                FullName = x["FullName"].AsString,
                GoalId = x["GoalId"].AsInt32,
                GoalTitle = x["GoalTitle"].AsString,
                StartDate = x["StartDate"].AsLocalTime,
                EndDate = x["EndDate"].AsLocalTime,
            }).ToList();
            
            return Ok(result);
        }
        
        /// <summary>
        /// Finder alle kommende skoleophold for en 
        /// </summary>
        /// <param name="elevId"></param>
        /// <returns>En liste af FutureSchoolDTO</returns>
        [HttpGet]
        [Route("future-schools/{elevId}")]
        public async Task<IActionResult> GetFutureSchools(int elevId)
        {
            if (elevId <= 0)
                return BadRequest("Forkert elevId");

            var goals = await _goalRepository.GetFutureSchools(elevId);

            List<FutureSchoolDTO> futureSchools = new ();
            
            foreach (var goal in goals)
            {
                futureSchools.Add(new FutureSchoolDTO
                {
                    Title = goal.Title,
                    SkoleNavn = goal.SkoleNavn,
                    SkoleStart = goal.StartDate,
                    SkoleEnd = goal.EndDate
                });
                
            }
            return Ok(futureSchools);
        }


        /// <summary>
        /// Starter goal med type = "Delmål" og sætter type = "InProgress" 
        /// </summary>
        /// <param name="bruger"></param>
        /// <returns>Goal</returns>
        [HttpPut]
        [Route("startgoal")]
        public async Task<IActionResult> StartGoal(MentorAssignment bruger)
        {
            if (bruger == null)
            {
                return BadRequest("Input er forkert");
            }
            
            var goal = await _goalRepository.StartGoal(bruger);

            if (goal == null)
            {
                return NotFound("Kunne ikke finde brugeren");
            }
            return Ok(goal);
        }

        /// <summary>
        /// Opdater goal til at være i status = "AwaitingApproval"
        /// </summary>
        /// <param name="bruger"></param>
        /// <returns>Goal</returns>
        [HttpPut]
        [Route("processgoal")]
        public async Task<IActionResult> ProcessGoal(MentorAssignment bruger)
        {
            if (bruger == null)
            {
                return BadRequest("Input er forkert");
            }
            var processedGoal = await _goalRepository.ProcessGoal(bruger);

            if (processedGoal == null)
            {
                return NotFound("Kunne ikke starte målet");
            }
            return Ok(processedGoal);
        }
        
        /// <summary>
        /// Sætter goal med status = Completed, og tjekker om hele forløbet er færdiggjort
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="forløbId"></param>
        /// <param name="goalId"></param>
        /// <returns>Retunerer et goal</returns>
        [HttpPut]
        [Route("confirmgoal/{planId}/{forløbId}/{goalId}")]
        public async Task<IActionResult> ConfirmGoal(int planId, int forløbId, int goalId)
        {
            if (planId <= 0 || forløbId <= 0 || goalId <= 0)
            {
                return BadRequest("Input er forkert");
            }
            
            var processedGoal = await _goalRepository.ConfirmGoalAndHandleProgress(planId, forløbId, goalId);

            if (processedGoal == null)
            {
                return NotFound("Kunne ikke færdiggøre målet eller forløbet");
            }
            
            return Ok(processedGoal);
        }
        
        /// <summary>
        /// Færdiggør et goal med type = Skoleophold og sætter status = Completed
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="forløbId"></param>
        /// <param name="goalId"></param>
        /// <returns>Opdateret goal</returns>
        [HttpPut]
        [Route("confirmschool/{planId}/{forløbId}/{goalId}")]
        public async Task<IActionResult> ConfirmSchoolOphold(int planId, int forløbId, int goalId)
        {
            if (planId <= 0 || forløbId <= 0 || goalId <= 0)
            {
                return BadRequest("Input er forkert");
            } 
            
             var result = await _goalRepository.UpdateSchoolStatus(planId, forløbId, goalId);

             if (result == null)
             {
                 return NotFound("Kunne ikke opdatere skolestatus");
             }
             
            return Ok(result);
        }

        /// <summary>
        /// Udregner, hvor mange mål pr. år en elev har og, hvor mange der er færdiggjort
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns>Ordered liste efter år.</returns>
        [HttpGet]
        [Route("progress/{studentId}")]
        public async Task<IActionResult> GetGoalProgress(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Input er forkert");                      
            }
            
            var forløbs = await _goalRepository.GetGoalsByStudentId(studentId);

            if (!forløbs.Any())
            {
                    return NotFound("Kunne ikke hente goalprogress");
            }

            var result = new List<GoalProgessDTO>();

            var semesters = forløbs
                .Select(f => f.Semester)
                .Distinct();

            foreach (var semester in semesters)
            {
                int totalGoals = 0;
                int completedGoals = 0;

                foreach (var forløb in forløbs.Where(f => f.Semester == semester))
                {
                    var goals = forløb.Goals;
                    totalGoals += goals.Count;
                    completedGoals += goals.Count(g => g.Status == "Completed");
                }

                result.Add(new GoalProgessDTO
                {
                    YearNumber = semester,
                    TotalGoals = totalGoals,
                    CompletedGoals = completedGoals
                });
            }

            result = result.OrderBy(r => r.YearNumber).ToList();

            return Ok(result);
        }
        
                
        /// <summary>
        /// Finder alle studerende, der mangler et kursus
        /// </summary>
        /// <param name="kursusCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allstudents/{kursusCode}")]
        public async Task<IActionResult> GetAllStudentsWithCourse(string kursusCode)
        {
            if (string.IsNullOrWhiteSpace(kursusCode))
            {
                return BadRequest("Kursus koden er blank");
            }
            
            List<KursusDeltagerListeDTO> students = new();
            var users = await _goalRepository.GetAllStudentsMissingCourse(kursusCode);
            
            foreach (var user in users)
            {
                students.Add(new KursusDeltagerListeDTO
                {
                    Id = user.Id,
                    Hotel = user.HotelNavn,
                    Navn = user.FirstName + "  " + user.LastName
                });
            }

            return Ok(students);
        }

        
        
    }
    
    

}