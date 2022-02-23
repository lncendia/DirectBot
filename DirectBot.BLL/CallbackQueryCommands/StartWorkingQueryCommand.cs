using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartWorkingQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        if (await serviceContainer.InstagramService.GetUserActiveInstagramsCountAsync(user.Id) == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет активированных аккаунтов.");
            return;
        }

        var subscribesCount = await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(user.Id);

        if (subscribesCount == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет подписок.");
            return;
        }

        user.CurrentWork = new WorkLiteDto();
        user.State = State.SelectAccounts;
        await serviceContainer.UserService.UpdateAsync(user);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите аккаунты:",
            replyMarkup: WorkingKeyboard.Select(
                (await serviceContainer.InstagramService.GetUserActiveInstagramsAsync(user.Id)).Take(subscribesCount)));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startWorking";
    }
}