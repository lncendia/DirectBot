namespace DirectBot.Core.Models;

public class InstagramDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? StateData { get; set; }
    public string? TwoFactorLoginInfo { get; set; }
    public string? ChallengeLoginInfo { get; set; }
    public bool IsActive { get; set; }
    public UserDTO User { get; set; } = null!;
    public ProxyDTO? Proxy { get; set; }
}