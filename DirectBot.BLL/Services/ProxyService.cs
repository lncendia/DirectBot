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

    public async Task<IOperationResult> SetProxyAsync(InstagramDto instagram)
    {
        var proxy = await _proxyRepository.GetRandomProxyAsync();
        if (proxy == null) return OperationResult.Fail("Proxy not found");
        instagram.Proxy = proxy;
        return await _instagramService.UpdateAsync(instagram);
    }

    public async Task<IOperationResult> DeleteProxyAsync(ProxyDto proxy)
    {
        try
        {
            await _proxyRepository.DeleteAsync(proxy);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    // public Task<IOperationResult> AddProxyAsync(string proxies)
    // {
    //     string[] proxyArray = proxies.Split('\n');
    //     foreach (var proxyString in proxyArray)
    //     {
    //         var data = proxyString.Split(":", 5);
    //         if (data.Length != 5) return false;
    //         if (!Org.BouncyCastle.Utilities.Net.IPAddress.IsValid(data[0])) return false;
    //         if (!int.TryParse(data[1], out int port)) return false;
    //         _db.Add(new Proxy()
    //             {Host = data[0], Port = port, Login = data[2], Password = data[3], Country = data[4].ToUpper()});
    //     }
    //
    //     _db.SaveChanges();
    //     return true;
    // }
}