using DirectBot.BLL.Interfaces;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class BillQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var payment = await serviceContainer.PaymentService.CheckPaymentAsync(query.Data![5..]);
        if (payment.Succeeded)
        {
            var subscribe = new SubscribeDTO
            {
                User = user!, EndSubscribe = DateTime.UtcNow.AddDays(30)
            };
            var result = await serviceContainer.SubscribeService.AddSubscribeAsync(subscribe);
            if (!result.Succeeded)
            {
                await client.AnswerCallbackQueryAsync(query.Id, "Произошла ошибка при добавлении подписки.");
                return;
            }

            var message = query.Message!.Text!;
            message = message.Replace("❌ Статус: Не оплачено", "✔ Статус: Оплачено");
            message = message.Remove(message.IndexOf("Оплачено", StringComparison.Ordinal) + 8);
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, message);
            await client.AnswerCallbackQueryAsync(query.Id, "Успешно оплачено.");
            return;
        }

        await client.AnswerCallbackQueryAsync(query.Id, "Не оплачено.");
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data!.StartsWith("bill");
    }
}