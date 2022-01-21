using DirectBot.Core.DTO;
using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramLoginService
{
    public Task<IResult<LoginResult>> ActivateAsync(InstagramDto instagram);
    public Task<IOperationResult> DeactivateAsync(InstagramDto instagram);
    public Task SendRequestsAfterLoginAsync(InstagramDto instagram);
    public Task<IResult<LoginTwoFactorResult>> EnterTwoFactorAsync(InstagramDto instagram, string code);
    public Task<IOperationResult> SubmitPhoneNumberAsync(InstagramDto instagram, string phoneNumber);
    public Task<IOperationResult> SmsMethodChallengeRequiredAsync(InstagramDto instagram);
    public Task<IOperationResult> EmailMethodChallengeRequiredAsync(InstagramDto instagram);
    public Task<IResult<ChallengeRequireVerifyMethod>> GetChallengeAsync(InstagramDto instagram);
    public Task<IResult<LoginResult>> SubmitChallengeAsync(InstagramDto instagram, string code);
}