namespace DirectBot.Core.Models;

public class InstagramDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? StateData { get; set; }
    public string? TwoFactorLoginInfo { get; set; }
    public string? ChallengeLoginInfo { get; set; }
    public bool IsActive { get; set; }
    public UserDto? User { get; set; }
    public ProxyDto? Proxy { get; set; }
}

public class InstagramLiteDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public bool IsActive { get; set; }
}