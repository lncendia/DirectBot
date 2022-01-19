using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService : IService<InstagramDTO>
{
    public Task<List<InstagramDTO>> GetUserInstagramsAsync(UserDTO user);
    public Task<int> GetUserInstagramsCountAsync(UserDTO user);
}