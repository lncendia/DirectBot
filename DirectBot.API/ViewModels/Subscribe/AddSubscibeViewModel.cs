using System.ComponentModel.DataAnnotations;

namespace DirectBot.API.ViewModels.Subscribe;

public class AddSubscribeViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "ID пользователя")]
    public long UserId { get; set; }
    

    [Display(Name = "Окончание подписки (в UTC)")]
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30).Date;
}