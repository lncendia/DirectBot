using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ContinueSelectQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var work = await serviceContainer.WorkService.GetUserSelectedWorkAsync(user!);
        if (work?.Instagrams == null || work.Instagrams.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не выбрали ни одного аккаунта.");
            return;
        }

        user!.State = State.EnterMassage;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите сообщение, которое хотите разослать:",
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "continueSelect" && user!.State == State.SelectAccounts;
    }
}