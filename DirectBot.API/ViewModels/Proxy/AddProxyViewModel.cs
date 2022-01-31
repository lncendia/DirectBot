using System.ComponentModel.DataAnnotations;

namespace DirectBot.API.ViewModels.Proxy;

public class AddProxyViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Список прокси")]
    [StringLength(5000, ErrorMessage = "Не более 5000 символов")]
    public string ProxyList { get; set; } = null!;
}