using System.ComponentModel.DataAnnotations;
using DirectBot.Core.Enums;

namespace DirectBot.API.ViewModels.User;

public class UserViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "ID")]
    public long Id { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Состояние")]
    public State State { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Админ")]
    public bool IsAdmin { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Заблокирован")]
    public bool IsBanned { get; set; }
}