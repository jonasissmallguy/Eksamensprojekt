using Core;
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

        [HttpPut("remove-student")]
        public async Task<IActionResult> RemoveStudent(int studentId, int kursusId)
        {
            var success = await _kursusRepository.RemoveStudentFromCourse(studentId, kursusId);
            if (success) return Ok();
            return NotFound();
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteCourse([FromBody] Kursus kursus)
        {
            var success = await _kursusRepository.CompleteCourse(kursus);
            if (success) return Ok();
            return BadRequest();
        }
    }
}