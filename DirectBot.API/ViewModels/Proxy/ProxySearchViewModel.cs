using System.ComponentModel.DataAnnotations;

namespace DirectBot.API.ViewModels.Proxy;

public class ProxySearchViewModel
{
    [Display(Name = "Хост")] public string? Host { get; set; }

    [Range(0, 65536)]
    [Display(Name = "Порт")]
    public int? Port { get; set; }

    [Display(Name = "Логин")] public string? Login { get; set; }
    [Display(Name = "Пароль")] public string? Password { get; set; }
    public int Page { get; set; } = 1;
}