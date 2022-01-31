using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IUserRepository : IRepository<UserDto, long>
{
    Task<int> GetCountAsync();
    Task<List<UserDto>> GetUsersAsync(UserSearchQuery query);
}