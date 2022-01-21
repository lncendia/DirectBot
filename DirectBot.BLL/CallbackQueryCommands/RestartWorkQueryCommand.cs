using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class RestartWorkQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![12..]);
        var work = await serviceContainer.WorkService.GetAsync(id);
        if (work == null || work.Instagram!.User!.Id != user.Id)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете перезапустить эту работу.");
            return;
        }

        var newWork = new WorkDto
        {
            Instagram = work.Instagram,
            Message = work.Message,
            LowerInterval = work.LowerInterval,
            UpperInterval = work.UpperInterval
        };

        var result = await serviceContainer.WorkService.AddAsync(newWork);
        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Ошибка: {result.ErrorMessage}.");
            return;
        }

        user.State = State.SelectTimeMode;
        await serviceContainer.UserService.UpdateAsync(user);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Выберите действие:",
            replyMarkup: WorkingKeyboard.StartWork);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("restartWork");
    }
}