using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IMailingService
{
    Task<IOperationResult> SendMessageAsync(InstagramDto instagram, Range delay, string message, InstaUser instaUser);
}