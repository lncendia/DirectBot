using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IUserService : IService<UserDto, long>
{
    Task<List<UserLiteDto>> GetAllAsync();
    Task<List<UserLiteDto>> GetUsersAsync(UserSearchQuery query);
}