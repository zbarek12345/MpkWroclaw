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
        public async Task<IActionResult> AddUser([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                UserModel um = new UserModel
                {
                    UserID = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    Username = registerRequest.Username,
                    Password = registerRequest.Password,
                    Name = registerRequest.Name,
                    Email = registerRequest.Email,
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

        [HttpGet("userData")]
        public async Task<IActionResult> GetUserData()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return StatusCode(401, "Authorization token is missing or invalid.");
            }

            if (!verifyGuid(authorizationHeader))
                return StatusCode(401);

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (!Guid.TryParse(token, out var parsedToken))
            {
                return StatusCode(401, "Invalid token format.");
            }

            return Ok(_userSingleton.getUserData(parsedToken));
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
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return StatusCode(401, "Authorization token is missing or invalid.");
            }

            if (!verifyGuid(authorizationHeader))
                return StatusCode(401);

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (!Guid.TryParse(token, out var parsedToken))
            {
                return StatusCode(401, "Invalid token format.");
            }

            var result = _userSingleton.updateFavorites(Guid.Parse(token_temp), favourites);

            if (result)
                return Ok();
            return StatusCode(201);
        }

        private bool TryGetToken(out Guid token)
        {
            token = Guid.Empty;
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return false;

            var tokenStr = authHeader.Substring("Bearer ".Length).Trim();
            if (!Guid.TryParse(tokenStr, out token) || !_userSingleton.VerifyToken(token))
                return false;

            return true;
        }

        [HttpGet("favorites")]
        public IActionResult GetFavorites()
        {
            if (!TryGetToken(out var token))
                return Unauthorized("Authorization token is missing or invalid.");

            var favoritesJson = _userSingleton.loadFavorites(token);
            return Ok(favoritesJson);
        }

        public class FavoriteDto
        {
            public string FavoriteId { get; set; }
        }

        [HttpPost("favorites")]
        public IActionResult AddFavorite([FromBody] FavoriteDto dto)
        {
            if (!TryGetToken(out var token))
                return Unauthorized("Authorization token is missing or invalid.");

            var result = _userSingleton.addFavorite(token, dto.FavoriteId);
            if (!result)
                return Conflict("Favorite already exists or failed to add.");

            return Ok("Favorite added successfully.");
        }

        [HttpDelete("favorites/{favoriteId}")]
        public IActionResult DeleteFavorite(string favoriteId)
        {
            if (!TryGetToken(out var token))
                return Unauthorized("Authorization token is missing or invalid.");

            var result = _userSingleton.deleteFavorite(token, favoriteId);
            if (!result)
                return NotFound("Favorite not found or failed to delete.");

            return Ok("Favorite deleted successfully.");
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