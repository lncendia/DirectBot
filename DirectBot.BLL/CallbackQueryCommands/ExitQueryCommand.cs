using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ExitQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![5..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User!.Id != user.Id)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете выйти из этого инстаграма.");
            return;
        }

        if (await serviceContainer.WorkService.HasActiveWorksAsync(instagram.Id))
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "На этом аккаунте есть незавершенные задачи.");
            return;
        }

        await client.SendChatActionAsync(user.Id, ChatAction.Typing);
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

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("exit");
    }
}