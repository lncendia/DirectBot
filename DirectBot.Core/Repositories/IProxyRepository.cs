using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IProxyRepository : IRepository<ProxyDto, int>
{
    public Task<ProxyDto?> GetRandomProxyAsync();
}