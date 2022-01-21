using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IProxyService
{
    public Task<IOperationResult> SetProxyAsync(InstagramDto instagram);
    public Task<IOperationResult> DeleteProxyAsync(ProxyDto proxy);
    // Task<IOperationResult> AddProxyAsync(string proxies);
}