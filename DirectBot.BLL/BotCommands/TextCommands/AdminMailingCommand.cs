using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class AdminMailingCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        if (user!.State is State.Main or State.SubscribesAdmin)
        {
            await client.SendTextMessageAsync(user.Id, "Введите сообщение, которое хотите разослать.",
                replyMarkup: MainKeyboard.Main);
            user!.State = State.MailingAdmin;
        }
        else
        {
            await client.SendTextMessageAsync(user.Id, "Вы вышли из панели рассылки.",
                replyMarkup: MainKeyboard.MainReplyKeyboard);
            user!.State = State.Main;
        }

        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text == "/mailing" &&
               user!.State is State.Main or State.MailingAdmin && user.IsAdmin;
    }
}