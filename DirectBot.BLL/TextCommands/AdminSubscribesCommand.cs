using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class AdminSubscribesCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (user.State == State.Main || user.State == State.MailingAdmin)
        {
            await client.SendTextMessageAsync(user.Id, "Добро пожаловать в панель подписок.",
                replyMarkup: new ReplyKeyboardRemove());
            await client.SendTextMessageAsync(user.Id,
                "Введите id человека и дату окончания подписки (111111111 11.11.2011).\nДля стандартного времени действия введите \"s\" (111111111 s).",
                replyMarkup: Keyboards.Main);
            user.State = State.SubscribesAdmin;
        }
        else
        {
            await client.SendTextMessageAsync(user.Id, "Вы вышли из панели рассылки.",
                replyMarkup: Keyboards.MainKeyboard);
            user.State = State.Main;
        }
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "/subscribes" && (user.State is State.Main or State.SubscribesAdmin) && BotSettings.Cfg.Admins.Contains(user.Id);
    }
}