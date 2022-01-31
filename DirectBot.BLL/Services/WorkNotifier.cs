using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.Services;

public class WorkNotifier : IWorkNotifier
{
    private readonly ITelegramBotClient _telegramBotClient;

    public WorkNotifier(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task<IOperationResult> NotifyStartAsync(WorkDto work)
    {
        if (work.Instagram?.User == null) return OperationResult.Fail("Отсутствуют необходимые данные.");
        try
        {
            await _telegramBotClient.SendTextMessageAsync(work.Instagram.User.Id, $"<b>Начато:</b>\n{work}",
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
        if (work.Instagram?.User == null) return OperationResult.Fail("Отсутствуют необходимые данные.");
        if (!work.IsCompleted) return OperationResult.Fail("Работа ещё не завершена.");
        try
        {
            await _telegramBotClient.SendTextMessageAsync(work.Instagram.User.Id, $"<b>Завершено:</b>\n{work}",
                ParseMode.Html);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}