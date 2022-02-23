using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface ISubscribeRepository : IRepository<SubscribeDto, int>
{
    Task<List<SubscribeDto>> GetUserSubscribesAsync(long id, int page);
    Task<List<SubscribeDto>> GetSubscribesAsync(SubscribeSearchQuery query);
    Task<int> GetUserSubscribesCountAsync(long id);
    Task<List<SubscribeDto>> GetExpiredSubscribes();
}