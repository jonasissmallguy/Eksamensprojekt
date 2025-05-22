using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server
{

    [ApiController]
    [Route("elevplan")]
    public class ElevplanController : ControllerBase
    {
        private ITemplateRepository _template;
        private IElevplan _elevplan;

        public ElevplanController(ITemplateRepository template, IElevplan elevplan)
        {
            _template = template;
            _elevplan = elevplan;
        }
        
        
        /// <summary>
        /// Returns a plan for a student
        /// </summary>
        /// <param name="studentId"></param>
        [HttpGet]
        [Route("{studentId:int}")]
        public async Task<IActionResult> GetElevplanByStudentId(int studentId)
        {
            var elevplan = await _elevplan.GetPlanByStudentId(studentId);

            if (elevplan == null)
            {
                return NotFound();
            }
            return Ok(elevplan);
        }

        /// <summary>
        /// Returns a Plan for a student with embedded Forløbs -> Goals -> Comments and returns a Plan
        /// </summary>
        /// <param name="studentId"></param>
        [HttpPost("{studentId:int}")]
        public async Task<IActionResult> CreateElevplan(int studentId)
        {
        
            var template = await _template.GetPlanTemplate(1);
            if (template == null)
            {
                return NotFound();  
            }

            var nyPlan = new Plan
            {
                StudentId = studentId,
                Title = template.Title,
                Description = template.Description,
                CreatedAt = DateTime.Now,
                Forløbs = new List<Forløb>()
            };
            

            foreach (var forløbTemplate in template.Forløbs)
            {
                var forløb = new Forløb
                {
                    Title = forløbTemplate.Title,
                    Semester = forløbTemplate.Semester,
                    Status =  "Active",
                    Goals = new List<Goal>()
                };

                foreach (var goalTemplate in forløbTemplate.Goals)
                {
                    var goal = new Goal
                    {
                        Type = goalTemplate.Type,
                        Title = goalTemplate.Title,
                        Description = goalTemplate.Description,
                        //Semester = forløb.Semester,
                        Status = "Active",
                        Comments = new List<Comment>()
                    };

                    forløb.Goals.Add(goal);
                }

                nyPlan.Forløbs.Add(forløb);
            }

            var elevplan = await _elevplan.SaveElevplan(studentId, nyPlan);

            if (elevplan == null)
            {
                return NotFound();
            }
            
            return Ok(nyPlan);
        }

    }
}