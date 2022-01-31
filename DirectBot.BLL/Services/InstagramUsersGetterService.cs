using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using DirectBot.BLL.Mapper;
using DirectBot.Core.DTO;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using InstagramApiSharp.WebApi;
using InstagramApiSharp.WebApi.Enums;

namespace DirectBot.BLL.Services;

public class InstagramUsersGetterService : IInstagramUsersGetterService
{
    private readonly IProxyService _proxyService;
    private readonly IFileDownloader _fileDownloader;

    public InstagramUsersGetterService(IProxyService proxyService, IFileDownloader fileDownloader)
    {
        _proxyService = proxyService;
        _fileDownloader = fileDownloader;
    }

    public async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersAsync(WorkDto workDto, CancellationToken token)
    {
        if (workDto.Instagram == null) throw new NullReferenceException();
        var api = await BuildApiAsync(workDto.Instagram);
        var delay = RequestDelay.FromSeconds(workDto.LowerInterval, workDto.UpperInterval);
        delay.Enable();
        api.SetRequestDelay(delay);
        return workDto.Type switch
        {
            WorkType.Subscriptions => await GetUsersFromFollowingAsync(api, token),
            WorkType.Subscribers => await GetUsersFromFollowersAsync(api, token),
            WorkType.Hashtag => await GetUsersFromHashtagAsync(api, workDto.Hashtag!, token),
            WorkType.File => await GetUsersFromFileAsync(api, workDto.FileIdentifier!, token),
            _ => throw new ArgumentOutOfRangeException()
        };
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

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFollowingAsync(IInstaApi api,
        CancellationToken token)
    {
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Following, 50,
            PaginationParameters.MaxPagesToLoad(10), token);
        return result.Succeeded
            ? Result<List<InstaUser>>.Ok(result.Value.Select(u => new InstaUser {Pk = u.Pk, Username = u.UserName})
                .ToList())
            : Result<List<InstaUser>>.Fail(result.Info.Message);
    }

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFollowersAsync(IInstaApi api,
        CancellationToken token)
    {
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Followers, 50,
            PaginationParameters.MaxPagesToLoad(10), token);
        return result.Succeeded
            ? Result<List<InstaUser>>.Ok(result.Value.Select(u => new InstaUser {Pk = u.Pk, Username = u.UserName})
                .ToList())
            : Result<List<InstaUser>>.Fail(result.Info.Message);
    }

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromHashtagAsync(IInstaApi api, string hashtag,
        CancellationToken token)
    {
        var resultRecent =
            await api.HashtagProcessor.GetRecentHashtagMediaListAsync(hashtag, PaginationParameters.MaxPagesToLoad(5));
        if (!resultRecent.Succeeded) return Result<List<InstaUser>>.Fail(resultRecent.Info.Message);
        var resultReels =
            await api.GetReelsHashtagMediaListAsync(hashtag, PaginationParameters.MaxPagesToLoad(5), token);
        if (!resultReels.Succeeded) return Result<List<InstaUser>>.Fail(resultReels.Info.Message);

        return Result<List<InstaUser>>.Ok(resultRecent.Value.Medias
            .UnionBy(resultReels.Value.Medias, media => media.User.Pk)
            .Select(u => new InstaUser {Pk = u.User.Pk, Username = u.User.UserName}).ToList());
    }


    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFileAsync(IInstaApi api, string fileId,
        CancellationToken token)
    {
        var result = await _fileDownloader.DownloadFileAsync(fileId, token);
        if (!result.Succeeded) return Result<List<InstaUser>>.Fail($"Не удалось получить файл: {result.Value}");

        await using var stream = result.Value!;
        var config = new CsvConfiguration(CultureInfo.GetCultureInfoByIetfLanguageTag("Ru"))
        {
            Delimiter = ";",
            HeaderValidated = null,
            MissingFieldFound = null
        };
        using var csv = new CsvReader(new StreamReader(stream), config);
        csv.Context.RegisterClassMap<InstaUserMapper>();
        try
        {
            var records = csv.GetRecords<InstaUser>().ToList();
            foreach (var user in records.Where(record => record.Pk == 0).ToList())
            {
                var instaUser = await api.UserProcessor.GetUserAsync(user.Username);
                if (instaUser.Succeeded) user.Pk = instaUser.Value.Pk;
                else records.Remove(user);
            }

            return !records.Any()
                ? Result<List<InstaUser>>.Fail("Не удалось получить пользователей")
                : Result<List<InstaUser>>.Ok(records.DistinctBy(record => record.Pk).ToList());
        }
        catch (Exception ex)
        {
            return Result<List<InstaUser>>.Fail($"Ошибка при разборе файла: {ex.Message}");
        }
    }
}