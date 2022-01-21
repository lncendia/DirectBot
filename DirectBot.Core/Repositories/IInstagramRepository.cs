using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IInstagramRepository : IRepository<InstagramDto, int>
{
    public Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user, bool onlyActive = false);
    public Task<int> GetUserInstagramsCountAsync(UserDto user, bool onlyActive = false);

    public Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page);
    public Task<InstagramDto?> GetUserSelectedInstagramAsync(UserDto userDto);
}