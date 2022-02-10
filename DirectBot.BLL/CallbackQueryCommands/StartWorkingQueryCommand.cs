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

        var userLite = serviceContainer.Mapper.Map<UserLiteDto>(user);
        if (await serviceContainer.InstagramService.GetUserActiveInstagramsCountAsync(userLite) == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет активированных аккаунтов.");
            return;
        }

        var subscribesCount = await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(userLite);

        if (subscribesCount == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет подписок.");
            return;
        }

        var work = new WorkDto
        {
            User = user
        };
        var result = await serviceContainer.WorkService.AddAsync(work);
        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id, $"Ошибка: {result.ErrorMessage}.");
            return;
        }

        user.CurrentWork = new WorkLiteDto {Id = work.Id};
        user.State = State.SelectAccounts;
        await serviceContainer.UserService.UpdateAsync(user);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите аккаунты:",
            replyMarkup: WorkingKeyboard.Select(
                (await serviceContainer.InstagramService.GetUserActiveInstagramsAsync(userLite))
                .Take(subscribesCount)));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startWorking";
    }
}