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
        var work = await serviceContainer.WorkService.GetUserSelectedWorkAsync(user!);
        if (work == null || work.Instagrams.Any())
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Работа отсутсвтует или не содержит инстаграм(ы).", replyMarkup: MainKeyboard.Main);
            return;
        }

        var result = await serviceContainer.WorkerService.StartWorkAsync(work);
        if (!result.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка при запуске работы: {result.ErrorMessage}.", replyMarkup: MainKeyboard.Main);
            return;
        }

        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Задача успешно запущена, вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startNow" && user!.State == State.SelectTimeMode;
    }
}