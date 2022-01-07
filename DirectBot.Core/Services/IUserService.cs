using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IUserService : IService<User>
{
    public Task<IResult<int>> GetCount();
}