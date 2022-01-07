using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SetOffsetQueryCommand:ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        await client.SendTextMessageAsync(user.Id, "Введите номер поста.", replyMarkup: Keyboards.Back("offsetSelect"));
        user.State = State.EnterOffset;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "setOffset" && user.State == State.SetOffset;
    }
}