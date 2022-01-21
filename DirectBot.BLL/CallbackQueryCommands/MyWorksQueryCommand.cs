using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MyWorksQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![13..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var work = await serviceContainer.WorkService.GetUserWorksAsync(user, page);
        if (work == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет работ.");
            return;
        }

        string workString =
            $"Работа №<code>{work.Id}</code>\nИнстаграм: <code>{work.Instagram!.Username}</code>\nСообщение: <code>{work.Message}</code>\nИнтервал: <code>{work.LowerInterval}:{work.UpperInterval}</code>\nЗавершена: <code>{(work.IsCompleted ? "Да" : "Нет")}</code>\n";
        if (work.IsCompleted)
            if (work.IsSucceeded)
                workString += $"Успешно: <code>Да</code>";
            else
                workString += $"Успешно: <code>Нет</code>\nОшибка: <code>{work.ErrorMessage}</code>";

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, workString, ParseMode.Html,
            replyMarkup: WorkingKeyboard.ActiveWorks(page, work));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("worksHistory");
    }
}