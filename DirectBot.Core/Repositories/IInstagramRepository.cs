using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IInstagramRepository : IRepository<InstagramDTO>
{
    public Task<List<InstagramDTO>> GetUserInstagramsAsync(UserDTO user);
    public Task<int> GetUserInstagramsCountAsync(UserDTO user);
}