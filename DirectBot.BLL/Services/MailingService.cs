using System.Net;
using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;

namespace DirectBot.BLL.Services;

public class MailingService : IMailingService
{
    private readonly IProxyService _proxyService;
    private readonly IInstagramService _instagramService;

    public MailingService(IProxyService proxyService, IInstagramService instagramService)
    {
        _proxyService = proxyService;
        _instagramService = instagramService;
    }

    public async Task<IOperationResult> SendMessageAsync(InstagramDto instagram, string message,
        long instaUserPk)
    {
        try
        {
            var api = await BuildApiAsync(instagram);
            var result = await api.MessagingProcessor.SendDirectTextAsync(instaUserPk.ToString(), null, message);
            if (result.Succeeded && result.Info.ResponseType == ResponseType.OK) return OperationResult.Ok();
            return OperationResult.Fail(result.Info.Message);
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    private async Task<IInstaApi> BuildApiAsync(InstagramDto instagram)
    {
        if (instagram.Proxy == null)
        {
            var instagramDto = await _instagramService.GetAsync(instagram.Id);
            if (instagramDto == null) throw new NullReferenceException("Не удалось получить инстаграм");
            await _proxyService.SetProxyAsync(instagramDto);
            instagram.Proxy = instagramDto.Proxy;
        }

        var builder = InstaApiBuilder.CreateBuilder();
        //.UseLogger(new DebugLogger(LogLevel.All));
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