using System.ComponentModel.DataAnnotations;

namespace DirectBot.Core.Configuration;

public class Configuration
{
    [Required(ErrorMessage = "The final channel is not set")]
    public string FinalChanel { get; set; } = null!;
}