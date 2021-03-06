using DirectBot.Core.Enums;

namespace DirectBot.DAL.Models;

public class Work
{
    public int Id { get; set; }
    public List<Instagram> Instagrams { get; set; } = new();

    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public WorkUsersType UsersType { get; set; }
    public WorkType Type { get; set; }
    public int CountUsers { get; set; }
    
    public int CountPerDivision { get; set; }
    public TimeSpan IntervalPerDivision { get; set; }

    public string? JobId { get; set; }
    public string? Hashtag { get; set; }
    public string? FileIdentifier { get; set; }
    
    public List<long> InstagramPks { get; set; } = new();
    
    
    public DateTime StartTime { get; set; }


    public bool IsCompleted { get; set; }
    //public bool IsCanceled { get; set; }
    public string? ErrorMessage { get; set; }


    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }
}