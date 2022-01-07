using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    public int GetCount();
}