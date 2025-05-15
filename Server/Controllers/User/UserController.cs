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

        

        [HttpPost]
        public async Task<IActionResult> PostUser(BrugerCreateDTO user)
        {

            var nyBruger = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                Email = user.Email,
                Rolle = user.Rolle,
                //mangler hotel
                StartDate = user.StartDate,
                Skole = user.Skole,
                Uddannelse = user.Uddannelse
            };
            
            Console.WriteLine($"User received: {user.FirstName}, {user.LastName}, {user.Email}");

            
             var newUser = await _userRepository.SaveBruger(nyBruger);
            
            return Ok(newUser);
        }

    }

}