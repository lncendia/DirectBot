using System.ComponentModel.DataAnnotations;

namespace DirectBot.Core.Configuration;

public class Project
{
    [Required(ErrorMessage = "Link is not set")]
    public string Link { get; set; } = null!;

    [Required(ErrorMessage = "Name is not set")]
    public string Name { get; set; } = null!;
}