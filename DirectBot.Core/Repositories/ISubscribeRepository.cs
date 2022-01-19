using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface ISubscribeRepository : IRepository<SubscribeDTO>
{
    Task<List<SubscribeDTO>> GetUserSubscribesAsync(UserDTO user, int page);
    Task<int> GetUserSubscribesCountAsync(UserDTO user);
}