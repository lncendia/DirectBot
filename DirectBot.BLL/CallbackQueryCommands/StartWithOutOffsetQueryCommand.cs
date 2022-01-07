using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartWithOutOffsetQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        user.State = State.SetTimeWork;
        await client.EditMessageTextAsync(query.From.Id,query.Message.MessageId,
            "Выбирете, когда хотите начать.", replyMarkup: Keyboards.StartWork);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "lastPost" && user.State == State.SetOffset;
    }
}