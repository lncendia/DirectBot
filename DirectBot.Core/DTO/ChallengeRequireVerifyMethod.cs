namespace DirectBot.Core.DTO;

public class ChallengeRequireVerifyMethod
{
    public bool SubmitPhoneRequired { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}