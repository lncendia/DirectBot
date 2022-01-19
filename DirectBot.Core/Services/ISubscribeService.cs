using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface ISubscribeService
{
    public Task<IResult<List<SubscribeDTO>>> GetUserSubscribesAsync(UserDTO user, int page);
    public Task<int> GetUserSubscribesCountAsync(UserDTO user);
    public Task<IOperationResult> AddSubscribeAsync(SubscribeDTO subscribe);
}