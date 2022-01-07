using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterLoginCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (user.Instagrams.Any(instagram => instagram.Username == message.Text))
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Вы уже добавили этот аккаунт.");
            user.State = State.Main;
            return;
        }

        user.CurrentInstagram.Username = message.Text;
        user.State = State.EnterPassword;
        await client.SendTextMessageAsync(message.Chat.Id,
            "Теперь введите пароль.", replyMarkup: Keyboards.Back("password"));
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.EnterLogin;
    }
}