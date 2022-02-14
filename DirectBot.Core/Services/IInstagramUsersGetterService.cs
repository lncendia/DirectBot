using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramUsersGetterService
{
     Task<IResult<List<long>>> GetUsersAsync(WorkDto workDto, CancellationToken token);
}