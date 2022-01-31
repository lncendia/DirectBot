using System.ComponentModel.DataAnnotations;

namespace DirectBot.API.ViewModels.Subscribe;

public class SubscribeSearchViewModel
{
    [Display(Name = "ID пользователя")] public long? UserId { get; set; }

    [Display(Name = "С")]
    [DataType(DataType.Date)]
    public DateTime? EndOfSubscribeLower { get; set; }

    [Display(Name = "До")]
    [DataType(DataType.Date)]
    public DateTime? EndOfSubscribeUpper { get; set; }

    public int Page { get; set; } = 1;
}