using AuthService.Models;
using AuthService.Services;
using AuthService.Services.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private const uint LOGIN_EXPIRATION_DAYS = 7;
    private const int MAX_FAILED_LOGIN_ATTEMPTS = 5;
    private const int HOURS_SINCE_LAST_FAIL = 6;
    
    private readonly UserService _users;
    private readonly ILogger<UserController> _logger;
    private readonly JwtGenerator _tokenGenerator;

    public UserController(UserService userService, ILogger<UserController> logger, JwtGenerator tokenGenerator)
    {
        _users = userService;
        _logger = logger;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUp signUpData)
    {
        await _users.CreateUser(signUpData);
        return Ok(new {Message = $"User {signUpData.Login} created"});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] SignIn signInData)
    {
        User u = await _users.GetUser(signInData.Login);

        TimeSpan timeSinceLastFail = u.LastFailedLogin is null
            ? TimeSpan.Zero
            : DateTime.UtcNow - u.LastFailedLogin.Value;
        
        if (u.FailedLoginCount >= MAX_FAILED_LOGIN_ATTEMPTS &&
            timeSinceLastFail.Hours < HOURS_SINCE_LAST_FAIL)
        {
            _logger.LogInformation("Too may login attempts for user {u.Login}", u.Login);
            return BadRequest(new {Message = "Too many login attempts, please try again later"});
        }
        
        if (!await _users.PasswordIsCorrect(u, signInData.Password))
        {
            await _users.FailedLogin(u);
            return BadRequest(new {Message = "Incorrect username or password"});
        }
        
        string token = _tokenGenerator.GenerateJwtKey(u, LOGIN_EXPIRATION_DAYS);
        CookieOptions cookieOpts = DefaultCookieOptions();
        cookieOpts.Expires = DateTime.UtcNow.AddDays(LOGIN_EXPIRATION_DAYS);
        Response.Cookies.Append("access_token", token, cookieOpts);
        await _users.SuccessLogin(u);
        
        return Ok(new
        {
            Token = token,
            Message = $"User {signInData.Login} signed in successfully!"
        });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        CookieOptions opts = DefaultCookieOptions();
        Response.Cookies.Delete("access_token", opts);
        return Ok("Good bye!");
    }
    
    private CookieOptions DefaultCookieOptions()
    {
        return new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
        };
    }
}