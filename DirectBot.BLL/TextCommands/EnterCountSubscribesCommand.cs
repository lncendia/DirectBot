using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterCountSubscribesCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (!int.TryParse(message.Text, out var count))
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Введите число!", replyMarkup: Keyboards.Main);
            return;
        }

        if (count > 100)
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Слишком большое количество!", replyMarkup: Keyboards.Main);
            return;
        }

        string billId = "";
        int bonus = count * BotSettings.Cfg.Bonus >= user.Bonus ? user.Bonus : count * BotSettings.Cfg.Bonus;
        var payUrl = new Payment().AddTransaction(count * BotSettings.Cfg.Cost - bonus, user, count, ref billId);
        if (payUrl == null)
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Произошла ошибка при создании счета. Попробуйте еще раз.", replyMarkup: Keyboards.Main);
            return;
        }

            
            
        user.Bonus -= bonus;
            
        user.State = State.Main;

        await client.SendTextMessageAsync(message.From.Id,
            $"💸 Оплата подписки на сумму {count * BotSettings.Cfg.Cost}₽ из которых {bonus}₽ списанно с бонусного счета.\n📆 Дата: {DateTime.Now:dd.MMM.yyyy}\n❌ Статус: Не оплачено.\n\n💳 Оплатите счет по ссылке.\n{payUrl}",
            replyMarkup: Keyboards.CheckBill(billId));
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.EnterCountToBuy;
    }
}