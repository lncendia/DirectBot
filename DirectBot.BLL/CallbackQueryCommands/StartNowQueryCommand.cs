using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
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
        var work = user!.CurrentWork == null
            ? null
            : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);
        if (work == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Работа отсутсвтует.", replyMarkup: MainKeyboard.Main);
            return;
        }

        var result = await serviceContainer.WorkerService.StartWorkNowAsync(work);
        if (!result.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка при запуске работы: {result.ErrorMessage}.", replyMarkup: MainKeyboard.Main);
            return;
        }

        user.CurrentWork = null;
        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Задача успешно запущена, вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startNow" && user!.State == State.SelectTimeMode;
    }
}