using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AuthService.Services;
using AuthService.Models;

namespace AuthService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _users;
        
        public UserController(UserService userService)
        {
            _users = userService;
        }

        [HttpGet("profile/{userId}")]
        public IActionResult GetProfile(string userId)
        {
            User u = _users.GetUser(userId).Result;
            return Ok(u);
        }

        [HttpGet("/")]
        public IActionResult GetOne()
        {
            return Ok("Test successfull");
        }
    }
}
