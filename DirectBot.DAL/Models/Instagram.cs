using DirectBot.Core.Models;

namespace DirectBot.DAL.Models;

public class Instagram
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? StateData { get; set; }
    public string? TwoFactorLoginInfo { get; set; }
    public string? ChallengeLoginInfo { get; set; }
    public bool IsActive { get; set; }
    public User User { get; set; } = null!;
    public Proxy? Proxy { get; set; }
    public List<Work>? Works { get; set; }
}