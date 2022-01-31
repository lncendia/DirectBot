using System.Net;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class ProxyParser : IProxyParser
{
    public IResult<List<ProxyDto>> GetProxies(string proxyList)
    {
        string[] proxyArray = proxyList.Split(Environment.NewLine);
        var list = new List<ProxyDto>();
        foreach (var proxyString in proxyArray)
        {
            var data = proxyString.Split(":", 4);
            if (!IPAddress.TryParse(data[0], out _)) return Result<List<ProxyDto>>.Fail("IP in the wrong format.");
            if (!int.TryParse(data[1], out var port)) return Result<List<ProxyDto>>.Fail("Port in the wrong format.");

            list.Add(new ProxyDto
            {
                Host = data[0], Port = port, Login = data[2], Password = data[3]
            });
        }

        return Result<List<ProxyDto>>.Ok(list);
    }
}