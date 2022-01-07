using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService<T>
{
    Task<IOperationResult> AddInstagram(Instagram instagram);
    Task<IOperationResult> DeleteInstagram(Instagram instagram);
    Task<IResult<bool>> EditInstagram(InstagramEditViewModel data, Instagram instagram);
    Task SaveData(Instagram instagram, IInstaApi instaApi);
    Task<IResult<InstaLoginResult>> ActivateAsync(Instagram instagram);
    Task SendRequestsAfterLoginAsync(Instagram instagram);
    IInstaApi BuildApi(Instagram instagram);
    Task<IResult<InstaLoginTwoFactorResult>> EnterTwoFactorAsync(Instagram instagram, string code);

    Task<IResult<InstaChallengeRequireSMSVerify>> SubmitPhoneNumberAsync(Instagram instagram,
        string phoneNumber);

    Task<IResult<InstaChallengeRequireSMSVerify>> SmsMethodChallengeRequiredAsync(Instagram instagram);

    Task<IResult<InstaChallengeRequireEmailVerify>> EmailMethodChallengeRequiredAsync(
        Instagram instagram);
    Task<IResult<InstaChallengeRequireVerifyMethod>> GetChallengeAsync(Instagram instagram);
    Task<IResult<InstaLoginResult>> SubmitChallengeAsync(Instagram instagram, string code);

}