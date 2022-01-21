using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IUserRepository : IRepository<UserDto, long>
{
    public Task<int> GetCountAsync();
}