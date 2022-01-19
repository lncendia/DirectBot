using DirectBot.Core.DTO;
using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramLoginService
{
    public Task<IResult<LoginResult>> ActivateAsync(InstagramDTO instagram);
    public Task<IOperationResult> DeactivateAsync(InstagramDTO instagram);
    public Task SendRequestsAfterLoginAsync(InstagramDTO instagram);
    public Task<IResult<LoginTwoFactorResult>> EnterTwoFactorAsync(InstagramDTO instagram, string code);
    public Task<IOperationResult> SubmitPhoneNumberAsync(InstagramDTO instagram, string phoneNumber);
    public Task<IOperationResult> SmsMethodChallengeRequiredAsync(InstagramDTO instagram);
    public Task<IOperationResult> EmailMethodChallengeRequiredAsync(InstagramDTO instagram);
    public Task<IResult<ChallengeRequireVerifyMethod>> GetChallengeAsync(InstagramDTO instagram);
    public Task<IResult<LoginResult>> SubmitChallengeAsync(InstagramDTO instagram, string code);
}