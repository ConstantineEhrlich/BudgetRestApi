using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    
}