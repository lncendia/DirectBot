using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class WorkCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите, что вы хотите сделать.", replyMarkup: Keyboards.Working);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "❤ Отработка" && user.State == State.Main;
    }
}