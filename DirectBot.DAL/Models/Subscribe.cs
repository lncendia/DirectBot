namespace DirectBot.DAL.Models;

public class Subscribe
{
    public int Id { get; set; }
    public long? UserId { get; set; }
    public User User { get; set; } = null!;
    public  DateTime EndSubscribe { get; set; }
}