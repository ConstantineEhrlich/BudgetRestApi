using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ProfileService _profiles;

    public ProfileController(ProfileService profiles)
    {
        _profiles = profiles;
    }

    [HttpGet("{profileLogin}")]
    public async Task<IActionResult> GetProfile(string profileLogin)
    {
        return Ok(await _profiles.GetProfile(profileLogin));
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] Profile updatedProfile)
    {
        await _profiles.UpdateProfile(updatedProfile);
        return Ok();
    }
}