using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.Services;

public class WorkNotifier : IWorkNotifier
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IInstagramService _instagramService;

    public WorkNotifier(ITelegramBotClient telegramBotClient, IInstagramService instagramService)
    {
        _telegramBotClient = telegramBotClient;
        _instagramService = instagramService;
    }

    private async Task<UserLiteDto?> GetWorksUser(WorkDto workDto)
    {
        if (!workDto.Instagrams.Any()) return null;
        var inst = await _instagramService.GetAsync(workDto.Instagrams.First().Id);
        return inst?.User;
    }

    public async Task<IOperationResult> NotifyStartAsync(WorkDto work)
    {
        var user = await GetWorksUser(work);
        if (user is null) return OperationResult.Fail("Не удалось получить пользователя.");
        try
        {
            await _telegramBotClient.SendTextMessageAsync(user.Id, $"<b>Начато:</b>\n{work}",
                ParseMode.Html,
                replyMarkup: WorkingKeyboard.StopWork(work.Id));
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> NotifyEndAsync(WorkDto work)
    {
        if (!work.IsCompleted) return OperationResult.Fail("Работа ещё не завершена.");
        var user = await GetWorksUser(work);
        if (user is null) return OperationResult.Fail("Не удалось получить пользователя.");
        try
        {
            await _telegramBotClient.SendTextMessageAsync(user.Id, $"<b>Завершено:</b>\n{work}",
                ParseMode.Html);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}