namespace AuthService.Models;

public class UserProfile
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
}