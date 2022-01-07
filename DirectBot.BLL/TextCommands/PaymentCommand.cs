using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class PaymentCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"Введите количество аккаунтов, которые хотите добавить. Цена одного аккаунта - {BotSettings.Cfg.Cost} рублей/30 дней.",
            replyMarkup: Keyboards.Main);
        user.State = State.EnterCountToBuy;
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "💰 Оплатить подписку" &&
               user.State == State.Main;
    }
}