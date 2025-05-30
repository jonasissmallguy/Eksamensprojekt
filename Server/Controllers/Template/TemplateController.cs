using Microsoft.AspNetCore.Mvc;

namespace Server
{

    [ApiController]
    [Route("template")]
    public class TemplateController : ControllerBase
    {
        
        
        private ITemplateRepository _templateRepository;

        public TemplateController(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }



        /// <summary>
        /// Henter en template
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retunerer en template</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var template = await _templateRepository.GetPlanTemplate(id);

            if (template == null)
            {
                return NotFound();
            }
            
            return Ok(template);
        }

        /// <summary>
        /// Henter alle templates
        /// </summary>
        /// <returns>En liste af templates</returns>
        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _templateRepository.GetAllPlanTemplates();
            
            return Ok(templates);
        }
    }

}