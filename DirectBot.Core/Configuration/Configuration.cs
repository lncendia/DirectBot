using System.ComponentModel.DataAnnotations;

namespace DirectBot.Core.Configuration;

public class Configuration
{
    [Required(ErrorMessage = "Bot token is not set")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Payment token is not set")]
    public string PaymentToken { get; set; } = null!;

    [Required(ErrorMessage = "Help address is not set")]
    public string HelpAddress { get; set; } = null!;

    [Required(ErrorMessage = "Instruction address is not set")]
    public string InstructionAddress { get; set; } = null!;

    [Required(ErrorMessage = "The subscribe cost is not set")]
    public decimal Cost { get; set; }
}