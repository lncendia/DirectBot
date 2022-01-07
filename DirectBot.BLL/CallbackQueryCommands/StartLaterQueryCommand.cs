using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartLaterQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        await client.DeleteMessageAsync(query.From.Id,
            query.Message.MessageId);
        user.State = State.SetDate;
        await client.SendTextMessageAsync(query.From.Id,
            "Введите время запуска по МСК в формате ЧЧ:мм. (<strong>Пример:</strong> <em>13:30</em>).",
            replyMarkup: Keyboards.Back("date"), parseMode: ParseMode.Html);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "startLater" && user.State == State.SetTimeWork;
    }
}