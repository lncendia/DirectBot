using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IProxyService : IService<ProxyDto, int>
{
    Task<List<ProxyDto>> GetProxiesAsync(ProxySearchQuery query);
    Task<int> GetProxiesCountAsync(ProxySearchQuery query);
    Task<IOperationResult> SetProxyAsync(InstagramDto instagram);
}