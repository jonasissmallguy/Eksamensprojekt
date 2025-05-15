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

        [HttpPost]
        public async Task<IActionResult> CreateElevplan(int studentId)
        {
            try
            {
                var template = await _template.GetPlanTemplate(1);
                if (template == null)
                {
                    return StatusCode(500, "Template was null");
                }

                var nyPlan = new Plan
                {
                    StudentId = studentId,
                    Title = template.Title,
                    Description = template.Description,
                    CreatedAt = DateTime.Now,
                    Forløbs = new List<Forløb>()
                };

                foreach (var forløbTemplate in template.Forløbs ?? new List<ForløbTemplate>())
                {
                    var forløb = new Forløb
                    {
                        Title = forløbTemplate.Title,
                        Semester = forløbTemplate.Semester,
                        Goals = new List<Goal>()
                    };

                    foreach (var goalTemplate in forløbTemplate.Goals ?? new List<GoalTemplate>())
                    {
                        var goal = new Goal
                        {
                            Type = goalTemplate.Type,
                            Title = template.Title,
                            Description = template.Description,
                            Semester = forløb.Semester,
                            Status = "Active",
                            Comments = new List<Comment>()
                        };

                        forløb.Goals.Add(goal);
                    }

                    nyPlan.Forløbs.Add(forløb);
                }

                await _elevplan.SaveElevplan(studentId, nyPlan);

                return Ok(nyPlan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

    }
}