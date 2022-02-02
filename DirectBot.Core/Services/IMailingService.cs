using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IMailingService
{
    Task<IOperationResult> SendMessageAsync(InstagramDto instagram, string message, InstaUser instaUser);
}