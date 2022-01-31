using DirectBot.API.ViewModels.Proxy;

namespace DirectBot.API.ViewModels.User;

public class UserListViewModel
{
    public UserSearchViewModel? UserSearchViewModel { get; set; }
    public List<UserViewModel>? Users { get; set; }
}