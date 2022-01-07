using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAccountsListQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (user.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        if (user.Instagrams.Count == 0 && user.Instagrams.All(_=>_.IsDeactivated))
        {
            await client.SendTextMessageAsync(user.Id,
                "У вас нет подходящих аккаунтов."); 
            return;
        }

        await client.SendTextMessageAsync(query.From.Id,
            "Нажмите на нужные аккаунты.", replyMarkup: Keyboards.Select(user));
        user.State = State.SelectAccounts;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "startWorking";
    }
}