namespace DirectBot.DAL.Models;

public class Payment
{
    public string Id { get; set; } = null!;
    public User User { get; set; } = null!;
    public long UserId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}