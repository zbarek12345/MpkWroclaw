using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;
using MPKWrocław.Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MPKWrocław.Controllers
{
    // class test
    // {
    //     [Test]
    //     void runTest()
    //     {
    //         UserController uc = new UserController(new UserSingleton());
    //         ClassicAssert.Null(uc.GetUserData());
    //     }
    // }
    
    
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
        public async Task<IActionResult> AddUser(string serialized)
        {
            try
            {   
                //using StreamReader streamReader = new StreamReader(serialized);
                //string json = await streamReader.ReadToEndAsync();
                var registerRequest = JsonSerializer.Deserialize<RegisterRequest>(serialized);
                UserModel um = new UserModel{
                    UserID = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
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
        
        [HttpGet("getUserData")]
        public async Task<IActionResult> GetUserData(string token_temp)
        {
            if (!verifyGuid(Request.Headers["Authorization"]))
                return StatusCode(401);
            return Ok(_userSingleton.getUserData(Guid.Parse(token_temp)));
        }
            
        private class userSetData{
            public String Username, Email;
        }

        [HttpPost("setUserData")]
        public async Task<IActionResult> SetUserData(String serializedData, string token_temp)
        {
            if (!verifyGuid(Request.Headers["Authorization"]))
                return StatusCode(401);
            
            userSetData data = JsonSerializer.Deserialize<userSetData>(serializedData);
            var token = Guid.Parse(Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim());
            _userSingleton.setUserData(Guid.Parse(token_temp), data.Username, data.Email);
            return Ok();
        }
        public class ChangePassword()
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        
        [HttpPost("SetUserPassword")]
        public async Task<IActionResult> SetUserPassword()
        {
            try
            {
                if (!verifyGuid(Request.Headers["Authorization"]))
                    return StatusCode(401);
                
                using StreamReader streamReader = new StreamReader(Request.Body);
                string json = await streamReader.ReadToEndAsync();
                
                var changePasswordRequest = JsonSerializer.Deserialize<ChangePassword>(json);
                
                var token = Guid.Parse(Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim());

                var success = _userSingleton.setPassword(token, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
        
                if (success)
                    return Ok();
        
                return StatusCode(401);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
    
        [HttpPost("SetUserFavorites")]
        public async Task<IActionResult> SetUserFavourites(string favourites,string token_temp)
        {
            if (!verifyGuid(Request.Headers["Authorization"]))
                return StatusCode(401);
            //var token = Guid.Parse(Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim());

            var result = _userSingleton.updateFavorites(Guid.Parse(token_temp), favourites);

            if (result)
                return Ok();
            return StatusCode(201);
        }

        [HttpGet("GetUserFavorites")]
        public async Task<IActionResult> GetUserFavorites(string token_temp)
        {
            if (!verifyGuid(Request.Headers["Authorization"]))
                return StatusCode(401);
            //var token = Guid.Parse(Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim());

            return Ok(_userSingleton.loadFavorites(Guid.Parse(token_temp)));
        }
        bool verifyGuid(string authHeader)
        {   
            return true;
            var guid = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                return _userSingleton.VerifyToken(Guid.Parse(guid));
            }
            catch (Exception e)
            {
                return false;
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
        public string Username{ get; set; }
        public string Name{ get; set; }
        public string Password{ get; set; }
        public string Email{ get; set; }
    }
}