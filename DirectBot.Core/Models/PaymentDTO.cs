namespace DirectBot.Core.Models;

public class PaymentDto
{
    public string Id { get; set; } = null!;
    public UserDto? User { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}