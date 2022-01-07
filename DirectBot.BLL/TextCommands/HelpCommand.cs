using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class HelpCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        user.State = State.Main;
        await client.SendTextMessageAsync(message.Chat.Id,
            "За поддержкой вы можете обратиться к @Per4at.");
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "🤝 Поддержка" && user.State != State.Block;
    }
}