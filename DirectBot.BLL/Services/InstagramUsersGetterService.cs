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
        return workDto.Type switch
        {
            WorkType.Subscriptions => await GetUsersFromFollowingAsync(api, workDto.CountUsers, token),
            WorkType.Subscribers => await GetUsersFromFollowersAsync(api, workDto.CountUsers, token),
            WorkType.Hashtag => await GetUsersFromHashtagAsync(api, workDto.Hashtag!, workDto.CountUsers, token),
            WorkType.File => await GetUsersFromFileAsync(api, workDto.FileIdentifier!, workDto.CountUsers, token),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<IInstaApi> BuildApiAsync(InstagramDto instagram)
    {
        if (instagram.Proxy == null) await _proxyService.SetProxyAsync(instagram);
        var builder = InstaApiBuilder.CreateBuilder();//.UseLogger(new DebugLogger(LogLevel.All)); //TODO: Remove logger
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

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFollowingAsync(IInstaApi api, int count,
        CancellationToken token)
    {
        double pageD = count / 50.0;
        int countPerPage = pageD >= 1 ? 50 : count, pageCount = (int) Math.Ceiling(pageD);
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Following,
            countPerPage,
            PaginationParameters.MaxPagesToLoad(pageCount), token);
        return result.Succeeded
            ? Result<List<InstaUser>>.Ok(result.Value.Take(count)
                .Select(u => new InstaUser {Pk = u.Pk, Username = u.UserName})
                .ToList())
            : Result<List<InstaUser>>.Fail(result.Info.Message);
    }

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFollowersAsync(IInstaApi api, int count,
        CancellationToken token)
    {
        double pageD = count / 50.0;
        int countPerPage = pageD >= 1 ? 50 : count, pageCount = (int) Math.Ceiling(pageD);
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Followers,
            countPerPage,
            PaginationParameters.MaxPagesToLoad(pageCount), token);
        return result.Succeeded
            ? Result<List<InstaUser>>.Ok(result.Value.Take(count)
                .Select(u => new InstaUser {Pk = u.Pk, Username = u.UserName})
                .ToList())
            : Result<List<InstaUser>>.Fail(result.Info.Message);
    }

    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromHashtagAsync(IInstaApi api, string hashtag,
        int count, CancellationToken token)
    {
        var pageCount = (int) Math.Ceiling(count / 30.0);
        var resultRecent =
            await api.GetRecentHashtagMediaListAsync(hashtag, PaginationParameters.MaxPagesToLoad(pageCount), token);
        var x = !resultRecent.Succeeded
            ? Result<List<InstaUser>>.Fail(resultRecent.Info.Message)
            : Result<List<InstaUser>>.Ok(resultRecent.Value.Medias.DistinctBy(media => media.User.Pk).Take(count)
                .Select(u => new InstaUser {Pk = u.User.Pk, Username = u.User.UserName}).ToList());
        return x;
    }


    private async Task<Core.Interfaces.IResult<List<InstaUser>>> GetUsersFromFileAsync(IInstaApi api, string fileId,
        int count, CancellationToken token)
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
            var records = csv.GetRecords<InstaUser>().Take(count).ToList();
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