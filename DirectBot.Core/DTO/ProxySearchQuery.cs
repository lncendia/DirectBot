namespace DirectBot.Core.DTO;

public class ProxySearchQuery
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public int Page { get; set; }
}