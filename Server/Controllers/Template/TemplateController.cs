using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers.Template
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
    }

}