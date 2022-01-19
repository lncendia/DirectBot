using System.ComponentModel.DataAnnotations;

namespace DirectBot.Core.Configuration;

public class Configuration
{
    [Required(ErrorMessage = "The bot token is not set")]
    public string Token { get; set; } = null!;
    
    [Required(ErrorMessage = "The payment token is not set")]
    public string PaymentToken { get; set; } = null!;

    [Required(ErrorMessage = "The subscribe cost is not set")]
    public decimal Cost { get; set; }
}