using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectLikeModeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        user.CurrentWorks.ForEach(x => x.SetMode(Mode.Like));
        user.State = State.SetHashtag;
        await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
            "Введите хештег без #.", replyMarkup: Keyboards.Back("selectMode"));
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "startLike" && user.State == State.SetMode;
    }
}