using Client;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    [ApiController]
    [Route("kursus")]
    public class KursusController : ControllerBase
    {
        private IKursus _kursusService;

        public KursusController(IKursus kursusService)
        {
            _kursusService = kursusService;
        }

        /// <summary>
        /// Henter alle kurser
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var kurser = await _kursusService.GetAllCourses();
            return Ok(kurser);
        }

        /// <summary>
        /// Henter et kursus baseret på ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var kursus = await _kursusService.GetCourseById(id);

            if (kursus == null)
                return NotFound();

            return Ok(kursus);
        }

        /// <summary>
        /// Fjerner en elev fra et kursus
        /// </summary>
        [HttpDelete]
        [Route("{kursusId}/student/{studentId}")]
        public async Task<IActionResult> RemoveStudent(int kursusId, int studentId)
        {
            var kursus = await _kursusService.GetCourseById(kursusId);
            if (kursus == null)
                return NotFound("Kursus ikke fundet");

            await _kursusService.RemoveStudentFromCourse(studentId, kursus);
            return Ok();
        }

        /// <summary>
        /// Marker et kursus som fuldført
        /// </summary>
        [HttpPost]