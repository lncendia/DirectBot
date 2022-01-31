using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IProxyRepository : IRepository<ProxyDto, int>
{
   Task<ProxyDto?> GetRandomProxyAsync();
   Task<List<ProxyDto>> GetProxiesAsync(ProxySearchQuery query);
}