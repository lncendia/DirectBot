namespace DirectBot.DAL.Models;

public class Proxy
{
    public int Id { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<Instagram> Instagrams { get; set; } = new();
}