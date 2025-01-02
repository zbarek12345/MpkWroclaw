using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;
using MPKWrocław.Models;

namespace MPKWrocław.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserSingleton _userSingleton;

        public UserController(UserSingleton userSingleton)
        {
            _userSingleton = userSingleton;
        }

        // POST api/user/login
        [HttpPost("login")]
        public ActionResult<Guid> Login([FromBody] LoginRequest loginRequest)
        {
            var token = _userSingleton.LoginUser(loginRequest.Username, loginRequest.Password);
            if (token == Guid.Empty)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(token);
        }

        // POST api/user/add
        [HttpPost("add")]
        public ActionResult AddUser([FromBody] UserModel userModel)
        {
            _userSingleton.AddUser(userModel);
            return Ok();
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}