namespace DirectBot.Core.Enums;

public enum State
{
    Main,
    EnterLogin,
    EnterPassword,
    EnterTwoFactorCode,
    ChallengeRequired,
    ChallengeRequiredAccept,
    ChallengeRequiredPhoneCall,
    SelectAccounts,
    SetMode,
    SetHashtag,
    SetDuration,
    SetTimeWork,
    SetOffset,
    EnterOffset,
    SetDate,
    EnterCountToBuy,
    Block,
    MailingAdmin,
    SubscribesAdmin
}