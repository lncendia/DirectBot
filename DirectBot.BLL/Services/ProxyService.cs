using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class ProxyService : IProxyService
{
    private readonly IProxyRepository _proxyRepository;
    private readonly IInstagramService _instagramService;

    public ProxyService(IProxyRepository proxyRepository, IInstagramService instagramService)
    {
        _proxyRepository = proxyRepository;
        _instagramService = instagramService;
    }

    public Task<List<ProxyDto>> GetProxiesAsync(ProxySearchQuery query) => _proxyRepository.GetProxiesAsync(query);
    
    public Task<int> GetProxiesCountAsync(ProxySearchQuery query) => _proxyRepository.GetProxiesCountAsync(query);

    public async Task<IOperationResult> SetProxyAsync(InstagramDto instagram)
    {
        var proxy = await _proxyRepository.GetRandomProxyAsync();
        if (proxy == null) return OperationResult.Fail("Proxy not found");
        instagram.Proxy = proxy;
        return await _instagramService.UpdateAsync(instagram);
    }

    public async Task<IOperationResult> DeleteAsync(ProxyDto entity)
    {
        try
        {
            await _proxyRepository.DeleteAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<ProxyDto?> GetAsync(int id) => _proxyRepository.GetAsync(id);

    public async Task<IOperationResult> UpdateAsync(ProxyDto entity)
    {
        try
        {
            await _proxyRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<IOperationResult> AddAsync(ProxyDto item) => UpdateAsync(item);
}