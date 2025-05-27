using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server
{

    [ApiController]
    [Route("elevplan")]
    public class ElevplanController : ControllerBase
    {
        private ITemplateRepository _templateRepository;
        private IElevplanRepository _elevplanRepository;

        public ElevplanController(ITemplateRepository template, IElevplanRepository elevplanRepository)
        {
            _templateRepository = template;
            _elevplanRepository = elevplanRepository;
        }
        
        
        /// <summary>
        /// Henter en elevplan
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns>En elevs plan, en fejl 400 eller en 404</returns>
        [HttpGet]
        [Route("{studentId:int}")]
        public async Task<IActionResult> GetElevplanByStudentId(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Forkert studentId");
            }
            
            var elevplan = await _elevplanRepository.GetPlanByStudentId(studentId);

            if (elevplan == null)
            {
                return NotFound("Kunne ikke finde elevplanen");
            }
            
            return Ok(elevplan);
        }

        /// <summary>
        /// Henter skabelon og generer en plan med midligertidige Id'er.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns>En ny Plan</returns>
        [HttpGet]
        [Route("gettemplate/{studentId:int}")]
        public async Task<IActionResult> GetTemplate(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Forkert studentId");
            }
            
            var template = await _templateRepository.GetPlanTemplate(1);
            
            if (template == null)
            {
                return NotFound("Kunne ikke finde skabelonen");  
            }

            var nyPlan = new Plan
            {
                StudentId = studentId,
                Title = template.Title,
                Description = template.Description,
                CreatedAt = DateTime.Now,
                Forløbs = new List<Forløb>()
            };

            int forløbIdCounter = -1;  
            int goalIdCounter = -1;

            foreach (var forløbTemplate in template.Forløbs)
            {
                var forløb = new Forløb
                {
                    Id = forløbIdCounter--,              
                    Title = forløbTemplate.Title,
                    Semester = forløbTemplate.Semester,
                    Status = "Active",
                    Goals = new List<Goal>()
                };

                foreach (var goalTemplate in forløbTemplate.Goals)
                {
                    var goal = new Goal
                    {
                        Id = goalIdCounter--,             
                        Type = goalTemplate.Type,
                        CourseCode = goalTemplate.CourseCode,
                        Title = goalTemplate.Title,
                        Description = goalTemplate.Description,
                        Status = "Active",
                        ForløbId = forløb.Id                
                    };

                    forløb.Goals.Add(goal);
                }

                nyPlan.Forløbs.Add(forløb);
            }
    
            return Ok(nyPlan);
        }


        /// <summary>
        /// Gemmer vores elevplan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="studentId"></param>
        /// <returns>UpdateResult</returns>
        [HttpPost]
        [Route("{studentId}")]
        public async Task<IActionResult> SaveElevplan([FromBody] Plan plan, int studentId)
        {
            if (plan == null || studentId <= 0)
            {
                return BadRequest("Forkert plan eller studentId");
            }
            
            var elevplan = await _elevplanRepository.SaveElevplan(studentId, plan);

            if (elevplan.MatchedCount == 0)
            {
                return NotFound();
            }

            if (elevplan.ModifiedCount == 0)
            {
                return BadRequest();
            }
            
            return Ok(elevplan);
        }

    }
}