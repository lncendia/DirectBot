using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterCountSubscribesCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        if (!int.TryParse(message.Text, out var count))
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Введите число!", replyMarkup: MainKeyboard.Main);
            return;
        }

        if (count > 100)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Слишком большое количество!", replyMarkup: MainKeyboard.Main);
            return;
        }

        var payment = await serviceContainer.PaymentService.CreateBillAsync(user!, count);
        if (payment.Succeeded)
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.From!.Id,
                $"💸 Оплата подписки на сумму {payment.Value!.Cost}₽.\n📆 Дата: {DateTime.Now:dd.MMM.yyyy}\n❌ Статус: Не оплачено.\n\n💳 Оплатите счет.",
                replyMarkup: PaymentKeyboard.CheckBill(payment.Value));
        }
        else
        {
            await client.SendTextMessageAsync(message.From!.Id,
                $"Произошла ошибка при создании счета ({payment.ErrorMessage}). Попробуйте еще раз.",
                replyMarkup: MainKeyboard.Main);
        }
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterCountToBuy;
    }
}