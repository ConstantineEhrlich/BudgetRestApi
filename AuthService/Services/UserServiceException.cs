namespace AuthService.Services;

public class UserServiceException: Exception
{
    public UserServiceException(string message) : base(message)
    {
    }
}