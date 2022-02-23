using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Logger;
using InstagramApiSharp.WebApi;
using InstagramApiSharp.WebApi.Enums;

namespace DirectBot.BLL.Services;

public class InstagramUsersGetterService : IInstagramUsersGetterService
{
    private readonly IProxyService _proxyService;
    private readonly IFileDownloader _fileDownloader;
    private readonly IInstagramService _instagramService;

    public InstagramUsersGetterService(IProxyService proxyService, IFileDownloader fileDownloader,
        IInstagramService instagramService)
    {
        _proxyService = proxyService;
        _fileDownloader = fileDownloader;
        _instagramService = instagramService;
    }

    public async Task<Core.Interfaces.IResult<List<long>>> GetUsersAsync(WorkDto workDto, CancellationToken token)
    {
        if (!workDto.Instagrams.Any()) return Result<List<long>>.Fail("Не удалось загрузить инстаграмы.");
        var instagram = await GetRandomInstagramAsync(workDto);
        if (instagram is not {IsActive: true})
            return Result<List<long>>.Fail("Не удалось получить инстаграм, выбранный для загрузки.");
        var api = await BuildApiAsync(instagram);
        return workDto.UsersType switch
        {
            WorkUsersType.Subscriptions => await GetUsersFromFollowingAsync(api, workDto.CountUsers, token),
            WorkUsersType.Subscribers => await GetUsersFromFollowersAsync(api, workDto.CountUsers, token),
            WorkUsersType.Hashtag => await GetUsersFromHashtagAsync(api, workDto.Hashtag!, workDto.CountUsers, token),
            WorkUsersType.File => await GetUsersFromFileAsync(api, workDto.FileIdentifier!, workDto.CountUsers, token),
            _ => Result<List<long>>.Fail("Неизвестный тип работы.")
        };
    }

    private async Task<InstagramDto?> GetRandomInstagramAsync(WorkDto work)
    {
        var instagramId = work.Instagrams[new Random().Next(work.Instagrams.Count)].Id;
        return await _instagramService.GetAsync(instagramId);
    }

    private async Task<IInstaApi> BuildApiAsync(InstagramDto instagram)
    {
        if (instagram.Proxy == null)
        {
            var instagramDto = await _instagramService.GetAsync(instagram.Id);
            if (instagramDto == null) throw new NullReferenceException();
            await _proxyService.SetProxyAsync(instagramDto);
            instagram.Proxy = instagramDto.Proxy;
        }

        var builder =
            InstaApiBuilder.CreateBuilder();
        builder.UseLogger(new DebugLogger(LogLevel.All)); //TODO: Remove logger
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

    private async Task<Core.Interfaces.IResult<List<long>>> GetUsersFromFollowingAsync(IInstaApi api,
        int count, CancellationToken token)
    {
        double pageD = count / 50.0;
        int countPerPage = pageD >= 1 ? 50 : count, pageCount = (int) Math.Ceiling(pageD);
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Following,
            countPerPage,
            PaginationParameters.MaxPagesToLoad(pageCount), token);
        return result.Succeeded
            ? Result<List<long>>.Ok(result.Value.Select(s => s.Pk).Take(count).ToList())
            : Result<List<long>>.Fail($"{api.GetLoggedUser().UserName}: {result.Info.Message}");
    }

    private async Task<Core.Interfaces.IResult<List<long>>> GetUsersFromFollowersAsync(IInstaApi api,
        int count, CancellationToken token)
    {
        double pageD = count / 50.0;
        int countPerPage = pageD >= 1 ? 50 : count, pageCount = (int) Math.Ceiling(pageD);
        var result = await api.GetUserFriendshipsAsync(api.GetLoggedUser().UserName, FriendshipStatus.Followers,
            countPerPage,
            PaginationParameters.MaxPagesToLoad(pageCount), token);
        return result.Succeeded
            ? Result<List<long>>.Ok(result.Value.Select(s => s.Pk).Take(count).ToList())
            : Result<List<long>>.Fail($"{api.GetLoggedUser().UserName}: {result.Info.Message}");
    }

    private async Task<Core.Interfaces.IResult<List<long>>> GetUsersFromHashtagAsync(IInstaApi api,
        string hashtag,
        int count, CancellationToken token)
    {
        var pageCount = (int) Math.Ceiling(count / 30.0);
        var resultRecent =
            await api.GetRecentHashtagMediaListAsync(hashtag, PaginationParameters.MaxPagesToLoad(pageCount), token);
        var x = !resultRecent.Succeeded
            ? Result<List<long>>.Fail($"{api.GetLoggedUser().UserName}: {resultRecent.Info.Message}")
            : Result<List<long>>.Ok(resultRecent.Value.Medias.Select(media => media.User.Pk).Distinct().Take(count)
                .ToList());
        return x;
    }


    private async Task<Core.Interfaces.IResult<List<long>>> GetUsersFromFileAsync(IInstaApi api, string fileId,
        int count, CancellationToken token)
    {
        var result = await _fileDownloader.DownloadFileAsync(fileId, token);
        if (!result.Succeeded) return Result<List<long>>.Fail($"Не удалось получить файл: {result.Value}");

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
                ? Result<List<long>>.Fail("Не удалось загрузить пользователей из файла")
                : Result<List<long>>.Ok(records.Select(record => record.Pk).Distinct().ToList());
        }
        catch (Exception ex)
        {
            return Result<List<long>>.Fail($"Ошибка при разборе файла: {ex.Message}");
        }
    }

    private class InstaUser
    {
        public long Pk { get; set; }
        public string? Username { get; set; }
    }

    private sealed class InstaUserMapper : ClassMap<InstaUser>
    {
        public InstaUserMapper()
        {
            Map(m => m.Pk).Name("Pk").Default(0);
            Map(m => m.Username).Name("Username");
        }
    }
}