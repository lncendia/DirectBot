using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class InstructionCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        user.State = State.Main;
        await client.SendTextMessageAsync(message.Chat.Id,
            "Всю инструкцию вы можете прочитать в канале @likebotgid.");
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "📄 Инструкция" && user.State != State.Block;
    }
}