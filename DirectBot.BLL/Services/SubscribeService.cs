using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class SubscribeService : ISubscribeService
{
    private readonly ISubscribeRepository _subscribeRepository;

    public SubscribeService(ISubscribeRepository subscribeRepository)
    {
        _subscribeRepository = subscribeRepository;
    }

    public Task<List<SubscribeDto>> GetUserSubscribesAsync(UserDto user, int page)
    {
        return _subscribeRepository.GetUserSubscribesAsync(user, page);
    }

    public Task<int> GetUserSubscribesCountAsync(UserDto user)
    {
        return _subscribeRepository.GetUserSubscribesCountAsync(user);
    }

    public async Task<IOperationResult> AddSubscribeAsync(SubscribeDto subscribe)
    {
        try
        {
            await _subscribeRepository.AddOrUpdateAsync(subscribe);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}