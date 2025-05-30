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

        /// <summary>
        /// Henter alle kurser
        /// </summary>
        /// <returns>En liste af kurser eller en fejl 404, hvis ingen kurser fundet</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kurser = await _kursusRepository.GetAllCourses();
            
            return Ok(kurser);
        }

        /// <summary>
        /// Henter et specifikt kursus
        /// </summary>
        /// <param name="kursusId">Id på kursus</param>
        /// <returns>Retunerer et kursus eller en fejl 404, hvis ingen kurser fundet</returns>
        [HttpGet("{kursusId}")]
        public async Task<IActionResult> GetById(int kursusId)
        {

            if (kursusId <= 0)
            {
                return BadRequest("Kursus id er ikke korrekt");
            }
            
            var kursus = await _kursusRepository.GetCourseById(kursusId);
            
            if (kursus == null)
            {
                return NotFound("Kunne ikke finde kursuet");
            }
            return Ok(kursus);
        }


        /// <summary>
        /// Fjerner en studerende fra kursus collection og herefter fra deres elevplan
        /// </summary>
        /// <param name="studentId">Id på den studerende</param>
        /// <param name="kursusCode">Kursuskoden, som vi ønsker at redigere</param>
        /// <returns>Opdateret kursus</returns>
        [HttpDelete]
        [Route("removestudent/{studentId}/{kursusCode}")]
        public async Task<IActionResult> RemoveStudent(int studentId, string kursusCode)
        {
            if (studentId <= 0 || kursusCode == null)
            {
                return BadRequest("Forkert studentId eller kursuskode");
            } 
            
            var kursus = await _kursusRepository.RemoveStudentFromCourse(studentId, kursusCode);

            if (kursus == null)
            {
                return NotFound("Kunne ikke finde eleven på kursuset");
            }
           
            var result = await _goalRepository.RemoveStudentFromACourse(studentId, kursusCode);

            if (result == false)
            {
                return NotFound("Kunne ikke fjerne kursuet fra elevens elevplan ");
            }
            
            return Ok(kursus);
        }
        
        /// <summary>
        /// Afslutter et kursus i hhv. kursus collection og på elevernes elevplaner
        /// </summary>
        /// <param name="kursusId"></param>
        /// <returns>retruner sand</returns>
        [HttpPut]
        [Route("complete/{kursusId}")]
        public async Task<IActionResult> CompleteCourse(int kursusId)
        {
            if (kursusId <= 0)
            {
                return BadRequest("Kursus id er ikke korrekt");
            }
            
            var success = await _kursusRepository.CompleteCourse(kursusId);

            if (success == null)
            {
                return NotFound("Kursus ikke fundet og færdiggjort");
            }
            
            List<int> allStudents = success.Students.Select(s => s.Id).ToList();
            var kursusCode = success.CourseCode;
            
            var result = await _goalRepository.CompleteAllStudentsOnCourse(allStudents, kursusCode);

            if (!result)
            {
                return NotFound("Kunne ikke sætte i alle elevplaner");
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Finder alle kursustemplates
        /// </summary>
        /// <returns>En liste af kursustemplates</returns>
        [HttpGet]
        [Route("templates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            var kursus = await _kursusRepository.GetAllTemplates();
            
            return Ok(kursus);
        }
        
        /// <summary>
        /// Tilføjer et kursus 
        /// </summary>
        /// <param name="kursus"></param>
        /// <returns>Et kursus</returns>
        [HttpPost]
        public async Task<IActionResult> AddCourse(KursusCreationDTO kursus)
        {
            if (kursus == null)
            {
                return BadRequest("Kursus objektet er ikke korrekt");
            }
            
            //Validering
            if (kursus.StartDate > kursus.EndDate)
            {
                return Conflict("Mismatch i start og slutdato");
            }
            
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
            
             var result = await _kursusRepository.SaveCourse(kursusModel);

             if (result == null)
             {
                 return BadRequest("Kunne ikke gemme kursuset");
             }
            
            return Ok(result);
        }

        
        /// <summary>
        /// Tilføjer en elev til kursuscollection og sætter i elevplanen  
        /// </summary>
        /// <param name="user"></param>
        /// <param name="kursusId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("addstudent/{kursusId}")]
        public async Task<IActionResult> AddStudentToCourse([FromBody] KursusDeltagerListeDTO user, int kursusId)
        {
            if (user == null || kursusId < 0)
            {
                return BadRequest("Kursus id er ikke korrekt eller brugeren");
            }
            
            var newParticipant = new User
            {
                Id = user.Id,
                FirstName = user.Navn,
                HotelNavn = user.Hotel,
            };
            
            var updateResult = await _kursusRepository.AddStudentToCourse(newParticipant, kursusId);

            if (updateResult == null)
            {
                return NotFound("Kunne ikke tildele eleven til krusuet");
            }

             var result = await _goalRepository.AddStudentToACourse(newParticipant.Id, updateResult);

             if (result == false)
             {
                 return NotFound("Kunne ikke tildele eleven til kursuet");
             }
            
            return Ok(updateResult);
        }

        /// <summary>
        /// Finder alle kurser indenfor et kvartal
        /// </summary>
        /// <returns>En liste af kommende kursuser som DTO</returns>
        [HttpGet]
        [Route("nextup")]
        public async Task<IActionResult> GetNextCommingCourses()
        {
            var comingCourses = await _kursusRepository.GetFutureCourses();

            if (comingCourses == null)
            {
                return NotFound("Kunne ikke finde nogen kommende kurser");
            } 
            List<KursusKommendeDTO> kursusListe = new();
            
            if (comingCourses != null)
            {
                foreach (var kursus in comingCourses)
                {
                    kursusListe.Add(new KursusKommendeDTO
                    {
                        KursusId = kursus.Id,
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

        /// <summary>
        /// Finder kursuser en elev er tilmeldt 
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns>En liste af tilmeldte kursuser som en DTO</returns>
        [HttpGet]
        [Route("nextup/{studentId}")]
        public async Task<IActionResult> GetNextCoursesByStudentId(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Kursus id er ikke korrekt");
            }
            
            var commingCourses = await _kursusRepository.GetFutureCourseByStudentId(studentId);

            if (commingCourses == null)
            {
                return NotFound("Kunne ikke finde nogen kurser for eleven");
            }
            
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