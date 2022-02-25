using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class SelectRestartModeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![14..]);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Укажите, необходимо ли получить пользоавтелей заново?", replyMarkup: WorkingKeyboard.Restart(id));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("selectRestart");
    }
}