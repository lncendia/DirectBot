using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IProxyParser
{
    IResult<List<ProxyDto>> GetProxies(string proxyList);
}