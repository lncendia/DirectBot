using DirectBot.Core.Enums;

namespace DirectBot.Core.Models;

public class UserDto
{
    public long Id { get; set; }
    public State State { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
    public InstagramLiteDto? CurrentInstagram { get; set; }
    public WorkLiteDto? CurrentWork { get; set; }
}

public class UserLiteDto
{
    public long Id { get; set; }
    public State State { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
}