using Client;
using Core;
using Core.DTO.Kursus;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    [ApiController]
    [Route("kursus")]
    public class KursusController : ControllerBase
    {
        private IKursusRepository _kursusRepository;
        private IGoalRepository _goalRepository; 

        public KursusController(IKursusRepository kursusRepository, IGoalRepository goalRepository)
        {
            _kursusRepository = kursusRepository;
            _goalRepository = goalRepository;
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
        [Route("removestudent/{studentId}/{kursusCode}")]
        public async Task<IActionResult> RemoveStudent(int studentId, string kursusCode)
        {
            var kursus = await _kursusRepository.RemoveStudentFromCourse(studentId, kursusCode);
            
            if (kursus == null) return BadRequest();
           
            var result = await _goalRepository.RemoveStudentFromACourse(studentId, kursusCode);
            
            if (result == false) return BadRequest();
            
            return Ok(kursus);
        }
        
        [HttpPut]
        [Route("complete/{kursusId}")]
        public async Task<IActionResult> CompleteCourse(int kursusId)
        {
            
            var success = await _kursusRepository.CompleteCourse(kursusId);
            
            List<int> allStudents = success.Students.Select(s => s.Id).ToList();
            var kursusCode = success.CourseCode;
            
            var result = await _goalRepository.CompleteAllStudentsOnCourse(allStudents, kursusCode);

            if (result == null)
            {
                return BadRequest();
            }
            
            return Ok(result);
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
                CourseCode = kursus.CourseCode,
                Title = kursus.Title,
                Location = kursus.Location,
                StartDate = kursus.StartDate,
                EndDate = kursus.EndDate,
                Description = kursus.Description,
                MaxParticipants = kursus.MaxParticipants,
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
            
            var updateResult = await _kursusRepository.AddStudentToCourse(newParticipant, kursusId);

            if (updateResult == null)
            {
                return NotFound();
            }
            
            
            //Tildeler kursus til eleven
            await _goalRepository.AddStudentToACourse(newParticipant.Id, updateResult);

            return Ok(updateResult);

        }

        [HttpGet]
        [Route("nextup")]
        public async Task<IActionResult> GetNextCommingCourses()
        {
            var comingCourses = await _kursusRepository.GetFutureCourses();
            
            if (comingCourses == null) return NotFound();

            List<KursusKommendeDTO> kursusListe = new();
            
            if (comingCourses != null)
            {
                foreach (var kursus in comingCourses)
                {
                    kursusListe.Add(new KursusKommendeDTO
                    {
                        CourseCode = kursus.CourseCode,
                        Title = kursus.Title,
                        Location = kursus.Location,
                        StartDate = kursus.StartDate,
                        Participants = kursus.Participants,
                        MaxParticipants = kursus.MaxParticipants
                    });
                }
            }
            return Ok(kursusListe);

        }

        [HttpGet]
        [Route("nextup/{studentId}")]
        public async Task<IActionResult> GetNextCoursesByStudentId(int studentId)
        {
            var commingCourses = await _kursusRepository.GetFutureCourseByStudentId(studentId);
            
            if (commingCourses == null) return NotFound();
            
            List<KursusKommendeDTO> kursusListe = new();

            foreach (var kursus in commingCourses)
            {
                kursusListe.Add(new KursusKommendeDTO
                {
                    CourseCode = kursus.CourseCode,
                    Title = kursus.Title,
                    Location = kursus.Location,
                    StartDate = kursus.StartDate
                    
                });
            }
            
            return Ok(kursusListe);
        }
        
    }
}