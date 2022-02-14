using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DirectBot.Core.Enums;

namespace DirectBot.DAL.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public State State { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }

    public int? CurrentInstagramId { get; set; }
    public int? CurrentWorkId { get; set; }
    public Instagram? CurrentInstagram { get; set; }
    public Work? CurrentWork { get; set; }

    public List<Subscribe> Subscribes { get; set; } = new();
    public List<Instagram>? Instagrams { get; set; } = new();
    public List<Payment>? Payments { get; set; } = new();
}