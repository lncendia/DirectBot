using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class BillQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var payment = await serviceContainer.BillService.GetPaymentAsync(query.Data![5..]);
        if (payment.Succeeded)
        {
            payment.Value!.User = serviceContainer.Mapper.Map<UserLiteDto>(user);
            var result = await serviceContainer.PaymentService.AddAsync(payment.Value);
            if (!result.Succeeded)
            {
                await client.AnswerCallbackQueryAsync(query.Id,
                    $"Произошла ошибка при добавлении платежа: {result.ErrorMessage}.");
            }

            var count = (int) (payment.Value!.Cost / serviceContainer.Configuration.Cost);
            for (var i = 0; i < count; i++)
            {
                var subscribe = new SubscribeDto
                {
                    User = serviceContainer.Mapper.Map<UserLiteDto>(user), EndSubscribe = DateTime.UtcNow.AddDays(30)
                };
                await serviceContainer.SubscribeService.AddAsync(subscribe);
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

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("bill");
    }
}