using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DirectBot.Core.Enums;

namespace DirectBot.Core.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }
    public List<Instagram>? Instagrams { get; set; }
    public List<Subscribe>? Subscribes { get; set; }
    public State State { get; set; }
    public List<Work>? CurrentWorks { get; set; }
    public Instagram? CurrentInstagram { get; set; }
}