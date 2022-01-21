using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.TextCommands;

public class AdminSubscribesCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        if (user!.State is State.Main or State.MailingAdmin)
        {
            await client.SendTextMessageAsync(user.Id, "Добро пожаловать в панель подписок.",
                replyMarkup: new ReplyKeyboardRemove());
            await client.SendTextMessageAsync(user.Id,
                "Введите id человека и дату окончания подписки (111111111 11.11.2011).\nДля стандартного времени действия введите \"s\" (111111111 s).",
                replyMarkup: MainKeyboard.Main);
            user!.State = State.SubscribesAdmin;
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
        return message.Type == MessageType.Text && message.Text == "/subscribes" &&
               user!.State is State.Main or State.SubscribesAdmin && user.IsAdmin;
    }
}