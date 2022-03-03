namespace DirectBot.API.ViewModels.User;

public class UserListViewModel
{
    public UserSearchViewModel? UserSearchViewModel { get; set; }
    public List<UserViewModel>? Users { get; set; }
    public int Count { get; set; }
}