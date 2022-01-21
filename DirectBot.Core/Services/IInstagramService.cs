using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService : IService<InstagramDto, int>
{
    public Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user);
    public Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page);
    public Task<List<InstagramDto>> GetUserActiveInstagramsAsync(UserDto user);
    public Task<int> GetUserInstagramsCountAsync(UserDto user);
    public Task<int> GetUserActiveInstagramsCountAsync(UserDto user);
    public Task<InstagramDto?> GetUserSelectedInstagramAsync(UserDto userDto);
}