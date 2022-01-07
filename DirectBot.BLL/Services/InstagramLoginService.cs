using System.Net;
using Newtonsoft.Json;

namespace DirectBot.BLL.Services;

public class InstagramLoginService : IInstagramService
{
    public readonly InstagramRepository;
    private readonly ProxyService _proxyService;

    public InstagramLoginService(ApplicationDbContext db, ReportService reportService, ProxyService proxyService)
    {
        _db = db;
        _reportService = reportService;
        _proxyService = proxyService;
    }

    public bool AddInstagram(InstagramLoginViewModel data, User user)
    {
        if (_db.Instagrams.Any(instagram1 => instagram1.Username == data.Username && instagram1.User == user))
            return false;
        var inst = new Instagram
        {
            Username = data.Username,
            Password = data.Password,
            Country = data.Country.ToUpper(),
            LikeChat = data.LikeChat,
            LikeChatType = data.LikeChatType,
            User = user
        };
        _db.Add(inst);
        _db.SaveChanges();
        return true;
    }

    public async Task<Interfaces.IResult<bool>> DeleteInstagram(Instagram instagram)
    {
        if (_db.Reports.Any(report => report.Instagrams.Contains(instagram) && !report.IsCompleted))
            return new Result<bool>(true, false, "На этом аккаунте в данный момент создаются отчёты.");
        try
        {
            var reports = _db.Reports
                .Where(report => report.Instagrams.Contains(instagram))
                .Include(report => report.ParticipantsReports).ThenInclude(report => report.UserPosts)
                .Include(report => report.MediaReports).ThenInclude(report => report.UserPosts)
                .Include(report => report.MediaReports).ThenInclude(report => report.Values).ToList();
            if (reports.Any(report => !_reportService.DeleteReport(report).Succeeded))
            {
                return new Result<bool>(true, false, "Не удалось удалить отчёты.");
            }

            if (instagram.IsActivated) await DeactivateAsync(instagram);
            _db.RemoveRange(_db.Participants.Where(participant => participant.Instagram == instagram));
            _db.Remove(instagram);
            await _db.SaveChangesAsync();
            return new Result<bool>(true, true, null);
        }
        catch (Exception ex)
        {
            return new Result<bool>(false, false, ex.Message);
        }
    }

    public async Task<Interfaces.IResult<bool>> EditInstagram(InstagramEditViewModel data, Instagram instagram)
    {
        if (_db.Reports.Any(report => report.Instagrams.Contains(instagram) && !report.IsCompleted))
            return new Result<bool>(true, false, "На этом аккаунте в данный момент создаются отчёты.");
        try
        {
            if (instagram.IsActivated) await DeactivateAsync(instagram);
            instagram.Username = data.Username;
            instagram.Password = data.Password;
            instagram.Country = data.Country;
            instagram.LikeChat = data.LikeChat;
            instagram.LikeChatType = data.LikeChatType;
            instagram.IsActivated = false;
            instagram.StateData = null;
            instagram.ChallengeLoginInfo = null;
            instagram.TwoFactorLoginInfo = null;
            instagram.Proxy = null;
            await _db.SaveChangesAsync();
            return new Result<bool>(true, true, null);
        }
        catch (Exception ex)
        {
            return new Result<bool>(false, false, ex.Message);
        }
    }

    public async Task SaveData(Instagram instagram, IInstaApi instaApi)
    {
        instagram.StateData = await instaApi.GetStateDataAsStringAsync();
        if (!ReferenceEquals(instaApi.ChallengeLoginInfo, null))
            instagram.ChallengeLoginInfo = JsonConvert.SerializeObject(instaApi.ChallengeLoginInfo);
        if (!ReferenceEquals(instaApi.TwoFactorLoginInfo, null))
            instagram.TwoFactorLoginInfo = JsonConvert.SerializeObject(instaApi.TwoFactorLoginInfo);
        await _db.SaveChangesAsync();
    }

    public async Task<IResult<InstaLoginResult>> ActivateAsync(Instagram instagram)
    {
        if (instagram.Proxy == null) _proxyService.SetProxy(instagram);
        var builder = InstaApiBuilder.CreateBuilder();
        //.UseLogger(new Logger(LogLevel.All, $"{instagram.Username}_{instagram.Id}.txt"));
        if (instagram.Proxy != null)
        {
            try
            {
                var webProxy = new WebProxy(instagram.Proxy.Host, instagram.Proxy.Port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(instagram.Proxy.Login, instagram.Proxy.Password)
                };
                builder = builder.UseHttpClientHandler(new HttpClientHandler { Proxy = webProxy });
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

        return data;
    }

    private async Task DeactivateAsync(Instagram instagram)
    {
        try
        {
            await BuildApi(instagram).LogoutAsync();
        }
        catch
        {
            // ignored
        }
    }

    public async Task SendRequestsAfterLoginAsync(Instagram instagram)
    {
        await BuildApi(instagram).SendRequestsAfterLoginAsync();
    }

    public IInstaApi BuildApi(Instagram instagram)
    {
        if (instagram.Proxy == null) _proxyService.SetProxy(instagram);
        var requestDelay = RequestDelay.FromSeconds(2, 3);
        requestDelay.Enable();
        var builder = InstaApiBuilder.CreateBuilder()
            .SetRequestDelay(requestDelay);
        // .UseLogger(new Logger(LogLevel.All, $"{instagram.Username}_{instagram.Id}.txt"));
        if (instagram.Proxy != null)
        {
            try
            {
                var webProxy = new WebProxy(instagram.Proxy.Host, instagram.Proxy.Port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(instagram.Proxy.Login, instagram.Proxy.Password)
                };
                builder = builder.UseHttpClientHandler(new HttpClientHandler { Proxy = webProxy });
            }
            catch
            {
                //ignored
            }
        }

        var instaApi = builder.Build();
        instaApi.LoadStateDataFromString(instagram.StateData);
        if (!string.IsNullOrEmpty(instagram.ChallengeLoginInfo))
            instaApi.ChallengeLoginInfo =
                JsonConvert.DeserializeObject<InstaChallengeLoginInfo>(instagram.ChallengeLoginInfo);

        if (!string.IsNullOrEmpty(instagram.TwoFactorLoginInfo))
            instaApi.TwoFactorLoginInfo =
                JsonConvert.DeserializeObject<InstaTwoFactorLoginInfo>(instagram.TwoFactorLoginInfo);
        return instaApi;
    }

    public async Task<IResult<InstaLoginTwoFactorResult>> EnterTwoFactorAsync(Instagram instagram, string code)
    {
        var api = BuildApi(instagram);
        var data = await api.TwoFactorLoginAsync(code);
        await SaveData(instagram, api);
        return data;
    }

    public async Task<IResult<InstaChallengeRequireSMSVerify>> SubmitPhoneNumberAsync(Instagram instagram,
        string phoneNumber)
    {
        return await BuildApi(instagram).SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
    }

    public async Task<IResult<InstaChallengeRequireSMSVerify>> SmsMethodChallengeRequiredAsync(Instagram instagram)
    {
        return await BuildApi(instagram).RequestVerifyCodeToSMSForChallengeRequireAsync();
    }

    public async Task<IResult<InstaChallengeRequireEmailVerify>> EmailMethodChallengeRequiredAsync(
        Instagram instagram)
    {
        return await BuildApi(instagram).RequestVerifyCodeToEmailForChallengeRequireAsync();
    }

    public async Task<IResult<InstaChallengeRequireVerifyMethod>> GetChallengeAsync(Instagram instagram)
    {
        return await BuildApi(instagram).GetChallengeRequireVerifyMethodAsync();
    }

    public async Task<IResult<InstaLoginResult>> SubmitChallengeAsync(Instagram instagram, string code)
    {
        var api = BuildApi(instagram);
        var data = await api.VerifyCodeForChallengeRequireAsync(code);
        await SaveData(instagram, api);
        return data;
    }
}