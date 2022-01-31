using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface ISubscribeService:IService<SubscribeDto, int>
{
     Task<List<SubscribeDto>> GetUserSubscribesAsync(UserDto user, int page);
     Task<List<SubscribeDto>> GetSubscribesAsync(SubscribeSearchQuery query);
     Task<List<SubscribeDto>> GetExpiredSubscribes();
     Task<int> GetUserSubscribesCountAsync(UserDto user);
}