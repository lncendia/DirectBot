using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IUserService : IService<UserDto, long>
{
    Task<int> GetCountAsync();
    Task<List<UserDto>> GetUsersAsync(UserSearchQuery query);
}