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

    public List<Subscribe>? Subscribes { get; set; }
    public List<Instagram>? Instagrams { get; set; }
    public List<Payment>? Payments { get; set; }
}