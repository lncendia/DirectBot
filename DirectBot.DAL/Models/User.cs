using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DirectBot.Core.Enums;

namespace DirectBot.DAL.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public List<DAL.Models.Instagram>? Instagrams { get; set; }
    public List<Subscribe>? Subscribes { get; set; }
    public State State { get; set; }
    public List<Work> CurrentWorks { get; set; } = null!;
    public DAL.Models.Instagram? CurrentInstagram { get; set; }
    public bool IsAdmin { get; set; }
}