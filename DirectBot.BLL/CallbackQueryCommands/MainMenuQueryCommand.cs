using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MainMenuQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var instagram = await serviceContainer.InstagramService.GetUserSelectedInstagramAsync(user!);
        if (instagram != null)
        {
            instagram.IsSelected = false;
            await serviceContainer.InstagramService.UpdateAsync(instagram);
        }

        foreach (var userCurrentWork in await serviceContainer.WorkService.GetUserActiveWorksAsync(user!))
        {
            var result = await serviceContainer.WorkService.DeleteAsync(userCurrentWork);
            if (result.Succeeded) continue;
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка: {result.ErrorMessage}", replyMarkup: MainKeyboard.Main);
            return;
        }

        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "mainMenu";
    }
}