using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IProxyService
{
    public Task<IOperationResult> SetProxyAsync(InstagramDTO instagram);
    public Task<IOperationResult> DeleteProxyAsync(ProxyDTO proxy);
    // Task<IOperationResult> AddProxyAsync(string proxies);
}