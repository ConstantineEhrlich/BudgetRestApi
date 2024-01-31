namespace AuthService.Services;

public class AuthServiceException: Exception
{
    public AuthServiceException(string message) : base(message)
    {
    }
}