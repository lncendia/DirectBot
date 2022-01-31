namespace DirectBot.Core.DTO;

public class SubscribeSearchQuery
{
    public long? UserId { get; set; }
    public DateTime? EndOfSubscribeUpper { get; set; }
    public DateTime? EndOfSubscribeLower { get; set; }
    public int Page { get; set; }
}