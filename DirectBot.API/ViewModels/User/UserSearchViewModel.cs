using System.ComponentModel.DataAnnotations;
using DirectBot.Core.Enums;

namespace DirectBot.API.ViewModels.User;

public class UserSearchViewModel
{
    [Display(Name = "ID")] public long? UserId { get; set; }
    [Display(Name = "Состояние")] public State? State { get; set; }
    [Display(Name = "Админ")] public bool IsAdmin { get; set; }
    [Display(Name = "Заблокирован")] public bool IsBanned { get; set; }
    public int Page { get; set; } = 1;
}