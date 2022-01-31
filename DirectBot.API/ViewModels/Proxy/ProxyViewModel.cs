namespace DirectBot.API.ViewModels.Proxy;

public class ProxyViewModel
{
    public int Id { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}