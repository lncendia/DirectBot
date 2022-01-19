namespace DirectBot.Core.Models;

public class SubscribeDTO
{
    public int Id { get; set; }
    public UserDTO User { get; set; } = null!;
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30);
}