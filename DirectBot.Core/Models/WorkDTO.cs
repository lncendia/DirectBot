namespace DirectBot.Core.Models;

public class WorkDto
{
    public int Id { get; set; }
    public InstagramDto? Instagram { get; set; }
    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? JobId { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
}