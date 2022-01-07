namespace DirectBot.Core.Models;

public class Subscribe
{
    public int Id { get; set; }
    public User User { get; set; } = null!;
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30);
}