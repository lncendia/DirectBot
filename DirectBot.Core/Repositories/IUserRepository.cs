using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IUserRepository : IRepository<UserDTO>
{
    public Task<int> GetCountAsync();
}