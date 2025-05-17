using Core;
using Microsoft.AspNetCore.Mvc;

namespace Server
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUsers =  await _userRepository.GetAllUsers();

            if (allUsers == null)
            {
                return NotFound();
            }
            return Ok(allUsers);
        }

        /// <summary>
        /// Henter alle bruger uden mig selv
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("withoutmyself/{id:int}")]
        public async Task<IActionResult> GetAllUsersWithOutMyself(int id)
        {
            var allUsers = await _userRepository.GetAllUsersWithOutMyself(id);

            if (allUsers == null)
            {
                return NotFound();
            }
            return Ok(allUsers);
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            
            return Ok(user);
        }

        
        //Opretter password
        public string GeneratePassword()
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var password = "";

            for (int i = 0; i < 8; i++)
            {
                var index = random.Next(chars.Length);
                password += chars[index];
            }

            return password;
        }
        

        /// <summary>
        /// Metode til at opret en bruger
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostUser(BrugerCreateDTO user)
        {
            string email = user.Email;
            
            var result = await _userRepository.CheckUnique(email);

            if (!result)
            {
                return Conflict("Du har en bruger");
            }
            
            var nyBruger = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                Email = user.Email,
                Password = GeneratePassword(),
                Rolle = user.Rolle,
                //mangler hotel
                Year = user.Year,
                StartDate = user.StartDate,
                EndDate = user.EndDate,
                Skole = user.Skole,
                Uddannelse = user.Uddannelse
            };
            
             var newUser = await _userRepository.SaveBruger(nyBruger);
            
            return Ok(newUser);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);

            return Ok();
        }

        /// <summary>
        /// Deaktiver en bruger der har status aktiv
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            await _userRepository.DeactivateUser(userId);
            
            return Ok();

        }
        
        /// <summary>
        /// Aktiver en bruger der er deaktiveret
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            await _userRepository.ActivateUser(userId);
            
            return Ok();
            
        }

        /// <summary>
        /// Ændrer rolle på en bruger
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updaterolle/{userId}/{newRolle}")]
        public async Task<IActionResult> UpdateRole(string newRolle, int userId)
        {
            await _userRepository.UpdateRolle(newRolle, userId);
            
            return Ok();
        }
        
        
        

    }

}