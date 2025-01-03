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
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                // Log raw JSON body for debugging
                Console.WriteLine($"Raw Body: {body}");

                try
                {
                    var userModel = JsonSerializer.Deserialize<UserModel>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ensures JSON property names are case-insensitive
                    });

                    if (userModel == null)
                    {
                        return BadRequest("Invalid JSON payload");
                    }

                    _userSingleton.AddUser(userModel);
                    return Ok(new { message = "User added successfully." });
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                    return BadRequest("Invalid JSON format.");
                }
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
}