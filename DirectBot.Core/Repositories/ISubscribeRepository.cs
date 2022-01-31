using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface ISubscribeRepository : IRepository<SubscribeDto, int>
{
    Task<List<SubscribeDto>> GetUserSubscribesAsync(UserDto user, int page);
    Task<List<SubscribeDto>> GetSubscribesAsync(SubscribeSearchQuery query);
    Task<int> GetUserSubscribesCountAsync(UserDto user);
    Task<List<SubscribeDto>> GetExpiredSubscribes();
}