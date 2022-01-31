using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IInstagramRepository : IRepository<InstagramDto, int>
{
    Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user, bool onlyActive = false);
    Task<int> GetUserInstagramsCountAsync(UserDto user, bool onlyActive = false);

    Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page);
    Task<InstagramDto?> GetUserSelectedInstagramAsync(UserDto userDto);
}