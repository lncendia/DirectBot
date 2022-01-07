using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class SendKeyboardCommand:ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        user.State = State.Main;
        await client.SendTextMessageAsync(message.From.Id,
            "Вы в главном меню.", replyMarkup:Keyboards.MainKeyboard);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text.StartsWith("/start") && user.State != State.Block;
    }
}