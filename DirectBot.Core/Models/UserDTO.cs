using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DirectBot.Core.Enums;

namespace DirectBot.Core.Models;

public class UserDTO
{
    public long Id { get; set; }
    public State State { get; set; }
    public List<WorkDTO> CurrentWorks { get; set; } = null!;
    public InstagramDTO? CurrentInstagram { get; set; }
    public bool IsAdmin { get; set; }
}