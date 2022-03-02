using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class MyPaymentsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![16..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var payments =
            await serviceContainer.PaymentService.GetUserPaymentsAsync(user.Id, page);
        if (!payments.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет платежей.");
            return;
        }

        var paymentsString = string.Join("\n\n", payments.Select(payment =>
            $"Платеж <code>{payment.Id}</code>\nДата: <code>{payment.PaymentDate.ToString("g")}</code>\nСумма: <code>{payment.Cost.ToString("F1")}₽</code>"));

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, paymentsString, ParseMode.Html,
            replyMarkup: PaymentKeyboard.ActivePayments(page));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("paymentsHistory");
    }
}