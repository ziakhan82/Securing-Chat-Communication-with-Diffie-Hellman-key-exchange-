using ChatServer.Repositories;
using CryptoChat.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Login/{userName}
        [HttpGet("{userName}")]
        public ActionResult<User> GetUserByName(string userName)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Name != userName);

            if (user == null)
            {
                return NotFound(); // Return 404 if user is not found
            }

            return Ok(user); // Return 200 with user details
        }

        [HttpGet("other/{userName}")]
        public ActionResult<User> GetOtherUserByYourName(string userName)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Name == userName);

            if (user == null)
            {
                return NotFound(); // Return 404 if user is not found
            }

            return Ok(user); // Return 200 with user details
        }

        // PUT: api/login/{userName}
        [HttpPut("{userName}")]
        public IActionResult UpdateUserPublicKey(string userName, [FromBody] int newPublicKey)
        {
            var existingUser = _userRepository.GetUsers().FirstOrDefault(u => u.Name == userName);

            if (existingUser == null)
            {
                return NotFound($"User with name '{userName}' not found.");
            }

            _userRepository.UpdateUserPublicKey(userName, newPublicKey);

            return Ok($"Public key for user '{userName}' updated successfully.");
        }

        // GET: api/Login/PublicSharedKey
        [HttpGet("PublicSharedKey")]
        public ActionResult<SecurityParameters> GetPublicSharedKey()
        {
            var publicParams = new SecurityParameters
            {
                Prime = 33, // This would be dynamically generated in a real-world scenario
                Generator = 7//// This would be dynamically generated in a real-world scenario
            };

            return Ok(publicParams);
        }
    }
}
