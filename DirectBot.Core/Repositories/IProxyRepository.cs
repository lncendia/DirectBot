using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IProxyRepository : IRepository<ProxyDTO>
{
    public Task<ProxyDTO?> GetRandomProxyAsync();
}