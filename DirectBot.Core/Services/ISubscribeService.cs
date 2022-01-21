using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface ISubscribeService
{
    public Task<List<SubscribeDto>> GetUserSubscribesAsync(UserDto user, int page);
    public Task<int> GetUserSubscribesCountAsync(UserDto user);
    public Task<IOperationResult> AddSubscribeAsync(SubscribeDto subscribe);
}