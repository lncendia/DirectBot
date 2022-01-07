namespace DirectBot.Core.Models;

public class Transaction
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public User User { get; set; } = null!;
    public DateTime DateTime { get; set; }
}