using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

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

        var work = await serviceContainer.WorkService.GetUserWorksAsync(user.Id, page);
        if (work == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет работ.");
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, work.ToString(), ParseMode.Html,
            replyMarkup: WorkingKeyboard.ActiveWorks(page, work));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("worksHistory");
    }
}