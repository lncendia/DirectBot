using DirectBot.Core.Enums;

namespace DirectBot.Core.DTO;

public class UserSearchQuery
{
    public long? UserId { get; set; }
    public State? State { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
    public int Page { get; set; }
}