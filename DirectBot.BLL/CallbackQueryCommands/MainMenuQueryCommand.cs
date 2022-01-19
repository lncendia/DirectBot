using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MainMenuQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        user!.CurrentInstagram = null;
        foreach (var userCurrentWork in user.CurrentWorks.ToList()) //TODO: Remove ToList() if use DTOs, cause EF Remove entity after DeleteAsync
        {
            var result = await serviceContainer.WorkService.DeleteAsync(userCurrentWork);
            if (result.Succeeded) continue;
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка: {result.ErrorMessage}");
            return;
        }

        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data == "mainMenu";
    }
}