using Core;
using Core.DTO.Kursus;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    [ApiController]
    [Route("kursus")]
    public class KursusController : ControllerBase
    {
        private readonly IKursusRepository _kursusRepository;

        public KursusController(IKursusRepository kursusRepository)
        {
            _kursusRepository = kursusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kurser = await _kursusRepository.GetAllCourses();
            return Ok(kurser);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kursus = await _kursusRepository.GetCourseById(id);
            if (kursus == null) return NotFound();
            return Ok(kursus);
        }

        [HttpDelete]
        [Route("removestudent/{studentId}/{kursusId}")]
        public async Task<IActionResult> RemoveStudent(int studentId, int kursusId)
        {
            var success = await _kursusRepository.RemoveStudentFromCourse(studentId, kursusId);
            return Ok(success);
        }


        

        [HttpPut]
        [Route("complete")]
        public async Task<IActionResult> CompleteCourse([FromBody] Kursus kursus)
        {
            
            
            var success = await _kursusRepository.CompleteCourse(kursus);
            
            return success ? Ok() : NotFound();
            
        }

        [HttpGet]
        [Route("templates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            var kursus = await _kursusRepository.GetAllTemplates();
            
            return Ok(kursus);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddCourse(KursusCreationDTO kursus)
        {
            var kursusModel = new Kursus
            {
                Title = kursus.Title,
                Location = kursus.Location,
                StartDate = kursus.StartDate,
                EndDate = kursus.EndDate
            };
            
            await _kursusRepository.SaveCourse(kursusModel);

            return Ok();
        }

        [HttpPut]
        [Route("addstudent/{kursusId}")]
        public async Task<IActionResult> AddStudentToCourse([FromBody] KursusDeltagerListeDTO user, int kursusId)
        {

            var newParticipant = new User
            {
                Id = user.Id,
                FirstName = user.Navn,
                HotelNavn = user.Hotel,
            };
            
            await _kursusRepository.AddStudentToCourse(newParticipant, kursusId);

            return Ok();

        }
        
    }
}