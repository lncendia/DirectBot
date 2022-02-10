namespace DirectBot.Core.Models;

public class SubscribeDto
{
    public int Id { get; set; }
    public UserLiteDto? User { get; set; }
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30);
}

