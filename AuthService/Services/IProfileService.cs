using AuthService.Models;

namespace AuthService.Services;

public interface IProfileService
{
    Profile GetProfile(User u);
    Profile UpdateProfile(Profile p);
}