using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class CancelWorkQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = long.Parse(query.Data![7..]);
        var work = await serviceContainer.WorkService.GetAsync(id);
        if (work == null || work.Instagram.User != user)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете отменить эту задачу.");
            return;
        }

        if (work.IsCompleted)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Задача уже завершена.");
        }
        else
        {
            var result = await serviceContainer.WorkerService.CancelWorkAsync(work);
            if (result.Succeeded) await client.AnswerCallbackQueryAsync(query.Id, "Задача успешно отменена.");
            else await client.AnswerCallbackQueryAsync(query.Id, $"Ошибка: {result.ErrorMessage}.", true);
        }
    }

    public bool Compare(CallbackQuery query, User? user)
    {
        return query.Data!.StartsWith("cancel");
    }
}