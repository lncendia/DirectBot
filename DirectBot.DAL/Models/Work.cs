using DirectBot.Core.Enums;

namespace DirectBot.DAL.Models;

public class Work
{
    public int Id { get; set; }

    public Instagram Instagram { get; set; } = null!;
    public int InstagramId { get; set; }

    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? JobId { get; set; }
    public string? Hashtag { get; set; }
    public string? FileIdentifier { get; set; }
    public WorkType Type { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }
}