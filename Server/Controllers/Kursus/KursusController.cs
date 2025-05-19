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

        [HttpDelete]
        [Route("removestudent/{studentId}/{kursusId}")]
        public async Task<IActionResult> RemoveStudent(int studentId, int kursusId)
        {
            var success = await _kursusRepository.RemoveStudentFromCourse(studentId, kursusId);
            return Ok(success);
        }

        /// <summary>
        /// Tilføjer en elev til et kursus og assigner i deres elevplan
        /// </summary>
        /// <param name="kursus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcourse/{studentId}")]
        public async Task AddStudentToCourse(int studentId, [FromBody] Kursus kursus)
        {
            
        }
        

        [HttpPut]
        [Route("complete")]
        public async Task<IActionResult> CompleteCourse([FromBody] Kursus kursus)
        {
            
            
            var success = await _kursusRepository.CompleteCourse(kursus);
            
            return success ? Ok() : NotFound();
            
        }
    }
}