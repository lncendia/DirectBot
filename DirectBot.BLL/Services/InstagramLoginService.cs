using System.Net;
using DirectBot.Core.DTO;
using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;

namespace DirectBot.BLL.Services;

public class InstagramLoginService : IInstagramLoginService
{
    private readonly IInstagramRepository _instagramRepository;
    private readonly IProxyService _proxyService;

    public InstagramLoginService(IProxyService proxyService, IInstagramRepository instagramRepository)
    {
        _proxyService = proxyService;
        _instagramRepository = instagramRepository;
    }


    private async Task SaveData(InstagramDTO instagram, IInstaApi instaApi)
    {
        instagram.StateData = await instaApi.GetStateDataAsStringAsync();
        if (!ReferenceEquals(instaApi.ChallengeLoginInfo, null))
            instagram.ChallengeLoginInfo = JsonConvert.SerializeObject(instaApi.ChallengeLoginInfo);
        if (!ReferenceEquals(instaApi.TwoFactorLoginInfo, null))
            instagram.TwoFactorLoginInfo = JsonConvert.SerializeObject(instaApi.TwoFactorLoginInfo);
        await _instagramRepository.UpdateAsync(instagram);
    }

    public async Task<Core.Interfaces.IResult<LoginResult>> ActivateAsync(InstagramDTO instagram)
    {
        if (instagram.Proxy == null) await _proxyService.SetProxyAsync(instagram);
        var builder = InstaApiBuilder.CreateBuilder();
        builder.UseLogger(new DebugLogger(LogLevel.All));
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

        var userSession = new UserSessionData
        {
            UserName = instagram.Username,
            Password = instagram.Password
        };
        var instaApi = builder.SetUser(userSession).Build();
        await instaApi.SendRequestsBeforeLoginAsync();
        var data = await instaApi.LoginAsync();
        await SaveData(instagram, instaApi);
        try
        {
            return Result<LoginResult>.Ok((LoginResult) Enum.Parse(typeof(LoginResult), data.Value.ToString()));
        }
        catch (ArgumentException)
        {
            return Result<LoginResult>.Fail("Unexpected result", LoginResult.Exception);
        }
    }

    public async Task<IOperationResult> DeactivateAsync(InstagramDTO instagram)
    {
        var result = await (await BuildApi(instagram)).LogoutAsync();
        if (!result.Succeeded) return OperationResult.Fail(result.Info.Message);
        return result.Value ? OperationResult.Ok() : OperationResult.Fail("Failed to log out of account");
    }

    public async Task SendRequestsAfterLoginAsync(InstagramDTO instagram)
    {
        await (await BuildApi(instagram)).SendRequestsAfterLoginAsync();
    }

    private async Task<IInstaApi> BuildApi(InstagramDTO instagram)
    {
        if (instagram.Proxy == null) await _proxyService.SetProxyAsync(instagram);
        var requestDelay = RequestDelay.FromSeconds(2, 3);
        requestDelay.Enable();
        var builder = InstaApiBuilder.CreateBuilder()
            .SetRequestDelay(requestDelay)
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
        if (!string.IsNullOrEmpty(instagram.ChallengeLoginInfo))
            instaApi.ChallengeLoginInfo =
                JsonConvert.DeserializeObject<InstaChallengeLoginInfo>(instagram.ChallengeLoginInfo);

        if (!string.IsNullOrEmpty(instagram.TwoFactorLoginInfo))
            instaApi.TwoFactorLoginInfo =
                JsonConvert.DeserializeObject<InstaTwoFactorLoginInfo>(instagram.TwoFactorLoginInfo);
        return instaApi;
    }

    public async Task<Core.Interfaces.IResult<LoginTwoFactorResult>> EnterTwoFactorAsync(InstagramDTO instagram, string code)
    {
        var api = await BuildApi(instagram);
        var data = await api.TwoFactorLoginAsync(code, 0);
        await SaveData(instagram, api);
        try
        {
            return Result<LoginTwoFactorResult>.Ok((LoginTwoFactorResult) Enum.Parse(typeof(LoginTwoFactorResult), data.Value.ToString()));
        }
        catch (ArgumentException)
        {
            return Result<LoginTwoFactorResult>.Fail("Unexpected result", LoginTwoFactorResult.Exception);
        }
    }

    public async Task<IOperationResult> SubmitPhoneNumberAsync(InstagramDTO instagram,
        string phoneNumber)
    {
        var result = await (await BuildApi(instagram)).SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
        return !result.Succeeded ? OperationResult.Fail(result.Info.Message) : OperationResult.Ok();
    }

    public async Task<IOperationResult> SmsMethodChallengeRequiredAsync(InstagramDTO instagram)
    {
        var result = await (await BuildApi(instagram)).RequestVerifyCodeToSMSForChallengeRequireAsync();
        return !result.Succeeded ? OperationResult.Fail(result.Info.Message) : OperationResult.Ok();
    }

    public async Task<IOperationResult> EmailMethodChallengeRequiredAsync(
        InstagramDTO instagram)
    {
        var result = await (await BuildApi(instagram)).RequestVerifyCodeToEmailForChallengeRequireAsync();
        return !result.Succeeded ? OperationResult.Fail(result.Info.Message) : OperationResult.Ok();
    }

    public async Task<Core.Interfaces.IResult<ChallengeRequireVerifyMethod>> GetChallengeAsync(InstagramDTO instagram)
    {
        var result = await (await BuildApi(instagram)).GetChallengeRequireVerifyMethodAsync();
        if (!result.Succeeded) return Result<ChallengeRequireVerifyMethod>.Fail(result.Info.Message);
        var challenge = new ChallengeRequireVerifyMethod
        {
            SubmitPhoneRequired = result.Value.SubmitPhoneRequired,
            Email = result.Value.StepData.Email,
            PhoneNumber = result.Value.StepData.PhoneNumber,
        };
        return Result<ChallengeRequireVerifyMethod>.Ok(challenge);
    }

    public async Task<Core.Interfaces.IResult<LoginResult>> SubmitChallengeAsync(InstagramDTO instagram, string code)
    {
        var api = await BuildApi(instagram);
        var data = await api.VerifyCodeForChallengeRequireAsync(code);
        await SaveData(instagram, api);
        try
        {
            return Result<LoginResult>.Ok((LoginResult) Enum.Parse(typeof(LoginResult), data.Value.ToString()));
        }
        catch (ArgumentException)
        {
            return Result<LoginResult>.Fail("Unexpected result", LoginResult.Exception);
        }
    }
}