using DirectBot.Core.DTO;
using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramLoginService
{
     Task<IResult<LoginResult>> ActivateAsync(InstagramDto instagram);
     Task<IOperationResult> DeactivateAsync(InstagramDto instagram);
     Task SendRequestsAfterLoginAsync(InstagramDto instagram);
     Task<IResult<LoginTwoFactorResult>> EnterTwoFactorAsync(InstagramDto instagram, string code);
     Task<IOperationResult> SubmitPhoneNumberAsync(InstagramDto instagram, string phoneNumber);
     Task<IOperationResult> SmsMethodChallengeRequiredAsync(InstagramDto instagram);
     Task<IOperationResult> EmailMethodChallengeRequiredAsync(InstagramDto instagram);
     Task<IResult<ChallengeRequireVerifyMethod>> GetChallengeAsync(InstagramDto instagram);
     Task<IResult<LoginResult>> SubmitChallengeAsync(InstagramDto instagram, string code);
}