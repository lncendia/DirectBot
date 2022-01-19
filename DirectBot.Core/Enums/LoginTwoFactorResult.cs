namespace DirectBot.Core.Enums;

public enum LoginTwoFactorResult
{
    Success,
    InvalidCode,
    CodeExpired,
    Exception,
    ChallengeRequired,
}