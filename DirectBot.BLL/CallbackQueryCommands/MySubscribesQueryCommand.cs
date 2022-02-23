using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MySubscribesQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![13..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var payments =
            await serviceContainer.SubscribeService.GetUserSubscribesAsync(user.Id, page);
        if (!payments.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет подписок.");
            return;
        }

        string paymentsString = string.Join("\n\n", payments.Select(payment =>
            $"Подписка №<code>{payment.Id}</code>\nДата окончания: <code>{payment.EndSubscribe.ToString("g")}</code>"));

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, paymentsString, ParseMode.Html,
            replyMarkup: PaymentKeyboard.ActiveSubscribes(page));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("mySubscribes");
    }
}