using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartNowQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var tasks = (await serviceContainer.WorkService.GetUserActiveWorksAsync(user!)).Select(work =>
        {
            work.JobId = serviceContainer.WorkerService.StartWork(work);
            return serviceContainer.WorkService.UpdateAsync(work);
        });
        await Task.WhenAll(tasks);
        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Задачи успешно запущены, вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startNow" && user!.State == State.SelectTimeMode;
    }
}