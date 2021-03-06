using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IMailingService
{
    Task<IOperationResult> SendMessageAsync(InstagramDto instagram, string message, long instaUserPk);
}