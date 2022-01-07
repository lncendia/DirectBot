using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class CancelWorkQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (user.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }
        Work work = user.Works.Find(x => x.Id == int.Parse(query.Data[7..]));
        if (work == null)
        {
            user.State = State.Main;
            await client.AnswerCallbackQueryAsync(query.Id, "Отработка не найдена.");
            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
            return;
        }

        if (work.IsStarted)
        {
            work.CancelTokenSource.Cancel();
        }
        else
        {
            await work.TimerDisposeAsync();
        }

        user.Works.Remove(work);
        await client.AnswerCallbackQueryAsync(query.Id, "Отработка отменена.");
        await client.DeleteMessageAsync(query.From.Id,
            query.Message.MessageId);
        user.State = State.Main;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("cancel");
    }
}