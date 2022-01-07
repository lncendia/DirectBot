using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterPasswordCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (message.Text.Length < 6)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пароль не может быть меньше 6 символов!");
            return;
        }
            
        user.CurrentInstagram.Password = message.Text;
        var login = await InstagramLoginService.ActivateAsync(user.CurrentInstagram);
        await MainBot.Login(user, login, db);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.EnterPassword;
    }
}