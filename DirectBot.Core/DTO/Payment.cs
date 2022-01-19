namespace DirectBot.Core.DTO;

public class Payment
{
    public string Id { get; set; } = null!;
    public string? PayUrl { get; set; }
    public decimal Cost { get; set; }
}