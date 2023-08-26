using System.Security.Claims;
using BudgetModel.Models;
using BudgetServices;
using BudgetWebApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;

    public UserController(ILogger<UserController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }
    
    
    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] Dto.SignUp payload)
    {
        try
        {
            User u = _userService.CreateUser(payload.Id, payload.Name, payload.Password);
            return Ok(new { Message = $"User {payload.Id} created successfully!"});
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Unexpected error occured"});
        }
    }


    [HttpPost("login")]
    public IActionResult SignIn([FromBody] Dto.SignIn payload)
    {
        // Check if user exists
        User u;
        try
        {
            u = _userService.GetUser(payload.UserId);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Unexpected error occured"});
        }
        
        TimeSpan timeSinceLastFail = new TimeSpan(0);
        if (u.LastFailedLogin is not null)
        {
            timeSinceLastFail = DateTime.Now - u.LastFailedLogin.GetValueOrDefault();
        }

        if (u.FailedLoginCount >= 5 && timeSinceLastFail.TotalHours < 6)
        {
            return BadRequest(new { Message = "Incorrect username or password" });
        }

        try
        {
            if (_userService.VerifyPassword(u, payload.Password))
            {
                _userService.SuccessLogin(u);
                string jwtToken = _userService.GenerateJwtKey(u);
                return Ok(new
                {
                    Token = jwtToken,
                    Message = $"User {payload.UserId} signed in successfully!"
                });
            }
            else
            {
                _userService.FailedLogin(u);
                return BadRequest(new { Message = "Incorrect username or password" });
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException.Message);
            return StatusCode(500, new { Message = "Unexpected error occured"});
        }
    }
    
    [HttpGet("profile/{userId?}")]
    public IActionResult GetProfile(string? userId)
    {
        Claim? loggedUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        if (loggedUserId is null)
        {
            return Unauthorized();
        }

        try
        {
            User u = _userService.GetUser(userId ?? loggedUserId.Value);
            return Ok(new UserDto(u));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Unexpected error occured"});
        }
    }
    
}