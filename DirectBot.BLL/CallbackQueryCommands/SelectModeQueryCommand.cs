using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectModeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (user.CurrentWorks.Count == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Вы не выбрали ни одного аккаунта.");
            return;
        }

        user.State = State.SetMode;
        await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        await client.SendTextMessageAsync(query.From.Id,
            "Выберите режим.", replyMarkup: Keyboards.SelectMode);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "selectMode" && user.State == State.SelectAccounts;
    }
}