using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.Services;

public class SubscribeNotifier : ISubscribeNotifier
{
    private readonly ISubscribeService _subscribeService;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<SubscribeNotifier> _logger;

    public SubscribeNotifier(ISubscribeService subscribeService, ITelegramBotClient telegramBotClient,
        ILogger<SubscribeNotifier> logger)
    {
        _subscribeService = subscribeService;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }

    public async Task NotifyStartAsync()
    {
        var subscribes = await _subscribeService.GetExpiredSubscribes();
        foreach (var subscribe in subscribes)
        {
            var result = await Task.WhenAll(NotifyAsync(subscribe), _subscribeService.DeleteAsync(subscribe));
            if (!result[1].Succeeded)
            {
                _logger.LogError("Error deleting subscribe {ErrorMessage}", result[1].ErrorMessage);
            }
        }
    }

    public void Trigger()
    {
        RecurringJob.Trigger("subscribesChecker");
    }

    private async Task<IOperationResult> NotifyAsync(SubscribeDto subscribeDto)
    {
        if (subscribeDto.User == null) return OperationResult.Fail("Отсутствуют необходимые данные.");
        string workString =
            $"Завершилась действие подписки №<code>{subscribeDto.Id}</code>\nПродлите её, нажав на кнопку.";

        try
        {
            await _telegramBotClient.SendTextMessageAsync(subscribeDto.User.Id, workString, ParseMode.Html,
                replyMarkup: PaymentKeyboard.PaySubscribe);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}