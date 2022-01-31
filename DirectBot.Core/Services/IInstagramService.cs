using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService : IService<InstagramDto, int>
{
     Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user);
     Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page);
     Task<List<InstagramDto>> GetUserActiveInstagramsAsync(UserDto user);
     Task<int> GetUserInstagramsCountAsync(UserDto user);
     Task<int> GetUserActiveInstagramsCountAsync(UserDto user);
     Task<InstagramDto?> GetUserSelectedInstagramAsync(UserDto userDto);
}