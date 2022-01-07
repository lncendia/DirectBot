using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartNowQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        await client.DeleteMessageAsync(query.From.Id,
            query.Message.MessageId);
        user.CurrentWorks.ForEach(async x => await x.StartAsync());
        user.CurrentWorks.Clear();
        user.State = State.Main;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "startNow" && user.State == State.SetTimeWork;
    }
}