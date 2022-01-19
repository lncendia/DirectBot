using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IUserService : IService<UserDTO>
{
    public Task<int> GetCountAsync();
}