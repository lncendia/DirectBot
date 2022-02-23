using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class SubscribeService : ISubscribeService
{
    private readonly ISubscribeRepository _subscribeRepository;
    public SubscribeService(ISubscribeRepository subscribeRepository) => _subscribeRepository = subscribeRepository;

    public Task<List<SubscribeDto>> GetUserSubscribesAsync(long userId, int page) =>
        _subscribeRepository.GetUserSubscribesAsync(userId, page);

    public Task<List<SubscribeDto>> GetSubscribesAsync(SubscribeSearchQuery query) =>
        _subscribeRepository.GetSubscribesAsync(query);

    public Task<List<SubscribeDto>> GetExpiredSubscribes() => _subscribeRepository.GetExpiredSubscribes();

    public Task<int> GetUserSubscribesCountAsync(long userId) =>
        _subscribeRepository.GetUserSubscribesCountAsync(userId);


    public async Task<IOperationResult> DeleteAsync(SubscribeDto subscribe)
    {
        try
        {
            await _subscribeRepository.DeleteAsync(subscribe);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
    
    public Task<SubscribeDto?> GetAsync(int id) => _subscribeRepository.GetAsync(id);

    public async Task<IOperationResult> UpdateAsync(SubscribeDto entity)
    {
        try
        {
            await _subscribeRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<IOperationResult> AddAsync(SubscribeDto item) => UpdateAsync(item);
}