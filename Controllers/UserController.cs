using System.Text.Json;
using System.Text.Json.Serialization;
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
            var token = _userSingleton.LoginUser(loginRequest.Username, loginRequest.Password, loginRequest.LogInDevice, loginRequest.LogInIp);
            if (token == Guid.Empty)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(token.ToString());
        }

// POST api/user/add
        [HttpPost("add")]
        public async Task<IActionResult> AddUser()
        {
            try
            {   
                using StreamReader streamReader = new StreamReader(Request.Body);
                string json = await streamReader.ReadToEndAsync();
                var registerRequest = JsonSerializer.Deserialize<RegisterRequest>(json);
                UserModel um = new UserModel{
                    UserID = Guid.Parse(registerRequest.UserID),
                    CreationDate = DateTime.Parse(registerRequest.CreationDate),
                    Username    =  registerRequest.Username,
                    Password    = registerRequest.Password,
                    Name = registerRequest.Name,
                    Email     = registerRequest.Email,
                };
                _userSingleton.AddUser(um);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string LogInDevice { get; set; }
        
        public string LogInIp { get; set; }
    }

    public class RegisterRequest
    {
        public string UserID{ get; set; }
        public string CreationDate{ get; set; }
        public string Username{ get; set; }
        public string Name{ get; set; }
        public string Password{ get; set; }
        public string Email{ get; set; }
    }
}