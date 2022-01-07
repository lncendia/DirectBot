using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class AdminMailingCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (user.State == State.Main || user.State == State.SubscribesAdmin)
        {
            await client.SendTextMessageAsync(user.Id, "Добро пожаловать в панель рассылки.",
                replyMarkup: new ReplyKeyboardRemove());
            await client.SendTextMessageAsync(user.Id,
                "Введите сообщение, которое хотите разослать.",
                replyMarkup: Keyboards.Main);
            user.State = State.MailingAdmin;
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
        return message.Type == MessageType.Text && message.Text == "/mailing" &&
               user.State is State.Main or State.MailingAdmin && BotSettings.Cfg.Admins.Contains(user.Id);
    }
}