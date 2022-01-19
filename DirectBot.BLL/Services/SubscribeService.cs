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

    public async Task<IResult<List<SubscribeDTO>>> GetUserSubscribesAsync(UserDTO user, int page)
    {
        try
        {
            var result = await _subscribeRepository.GetUserSubscribesAsync(user, page);
            return Result<List<SubscribeDTO>>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<List<SubscribeDTO>>.Fail(ex.Message);
        }
    }

    public Task<int> GetUserSubscribesCountAsync(UserDTO user)
    {
        return _subscribeRepository.GetUserSubscribesCountAsync(user);
    }

    public async Task<IOperationResult> AddSubscribeAsync(SubscribeDTO subscribe)
    {
        try
        {
            await _subscribeRepository.AddAsync(subscribe);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}