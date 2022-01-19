using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartNowQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        user!.CurrentWorks.ForEach(work => work.JobId = serviceContainer.WorkerService.StartWork(work));
        user.State = State.Main;
        user.CurrentWorks.Clear();
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Задачи успешно запущены, вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data == "startNow" && user!.State == State.SelectTimeMode;
    }
}