namespace DirectBot.Core.Models;

public class WorkDTO
{
    public int Id { get; set; }
    public InstagramDTO Instagram { get; set; } = null!;
    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? JobId { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSucceeded { get; set; }
}