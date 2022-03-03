namespace DirectBot.API.ViewModels.Subscribe;

public class SubscribeListViewModel
{
    public SubscribeSearchViewModel? SubscribeSearchViewModel { get; set; }
    public List<SubscribeViewModel>? Subscribes { get; set; }
    public int Count { get; set; }
}