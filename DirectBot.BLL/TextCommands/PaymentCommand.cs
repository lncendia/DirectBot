using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class PaymentCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"Введите количество аккаунтов, которые хотите добавить. Цена одного аккаунта - {serviceContainer.Configuration.Cost} рублей/30 дней.",
            replyMarkup: MainKeyboard.Main);
        user!.State = State.EnterCountToBuy;
        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && message.Text == "💰 Оплатить подписку" &&
               user!.State == State.Main;
    }
}