using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ExitQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = long.Parse(query.Data![5..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User != user)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете выйти из этого инстаграма.");
            return;
        }

        if (await serviceContainer.WorkService.HasActiveWorksAsync(instagram))
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "На этом аккаунте есть незавершенные задачи.");
            return;
        }

        if (instagram.IsActive) await serviceContainer.InstagramLoginService.DeactivateAsync(instagram);
        var result = await serviceContainer.InstagramService.DeleteAsync(instagram);

        if (result.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Аккаунт успешно удалён.");
        }
        else
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка при удалении аккаунта ({result.ErrorMessage}).");
        }
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data!.StartsWith("exit");
    }
}