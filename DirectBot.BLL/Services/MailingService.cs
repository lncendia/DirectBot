using System.Net;
using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using DirectBot.DAL.Models;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;

namespace DirectBot.BLL.Services;

public class MailingService : IMailingService
{
    private readonly IProxyService _proxyService;

    public MailingService(IProxyService proxyService) => _proxyService = proxyService;

    public async Task<IOperationResult> SendMessageAsync(InstagramDto instagram, Range delay, string message,
        InstaUser instaUser)
    {
        var api = await BuildApiAsync(instagram);
        var apiDelay = RequestDelay.FromSeconds(delay.Start.Value, delay.End.Value);
        apiDelay.Enable();
        api.SetRequestDelay(apiDelay);
        var result = await api.MessagingProcessor.SendDirectTextAsync(instaUser.Pk.ToString(), null, message);
        if (result.Succeeded && result.Info.ResponseType == ResponseType.OK) return OperationResult.Ok();
        return OperationResult.Fail(result.Info.Message);
    }

    private async Task<IInstaApi> BuildApiAsync(InstagramDto instagram)
    {
        if (instagram.Proxy == null) await _proxyService.SetProxyAsync(instagram);
        var builder = InstaApiBuilder.CreateBuilder()
            .UseLogger(new DebugLogger(LogLevel.All));
        if (instagram.Proxy != null)
        {
            try
            {
                var webProxy = new WebProxy(instagram.Proxy.Host, instagram.Proxy.Port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(instagram.Proxy.Login, instagram.Proxy.Password)
                };
                builder = builder.UseHttpClientHandler(new HttpClientHandler {Proxy = webProxy});
            }
            catch
            {
                //ignored
            }
        }

        var instaApi = builder.Build();
        await instaApi.LoadStateDataFromStringAsync(instagram.StateData);
        return instaApi;
    }
}