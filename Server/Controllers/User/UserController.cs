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
                StartDate = user.StartDate,
                Skole = user.Skole,
                Uddannelse = user.Uddannelse
            };
            
             var newUser = await _userRepository.SaveBruger(nyBruger);
            
            return Ok(newUser);
        }

    }

}