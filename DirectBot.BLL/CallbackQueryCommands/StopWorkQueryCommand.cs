using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StopWorkQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![9..]);
        var work = await serviceContainer.WorkService.GetAsync(id);
        if (work == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете остановить эту работу.");
            return;
        }


        var result = await serviceContainer.WorkerService.CancelWorkAsync(work);
        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Ошибка: {result.ErrorMessage}.");
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Работа успешно остановлена.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("stopWork");
    }
}