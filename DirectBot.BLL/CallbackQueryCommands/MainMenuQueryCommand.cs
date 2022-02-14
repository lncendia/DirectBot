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
        var work = user!.CurrentWork == null
            ? null
            : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);
        if (work != null)
        {
            var result = await serviceContainer.WorkService.DeleteAsync(work);
            if (!result.Succeeded)
            {
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    $"Ошибка: {result.ErrorMessage}", replyMarkup: MainKeyboard.Main);
                return;
            }
        }
        user.CurrentWork = null;
        user.CurrentInstagram = null;
        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "mainMenu";
    }
}