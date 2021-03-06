using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class SelectAllAccountsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var subscribesCount = await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(user!.Id);
        if (subscribesCount == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет подписок.");
            return;
        }

        var work = user.CurrentWork == null
            ? null
            : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);
        if (work == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Работа не выбрана. Попробуйте ещё раз.");
            return;
        }

        var instagrams =
            (await serviceContainer.InstagramService.GetUserActiveInstagramsAsync(user.Id))
            .Where(dto => work.Instagrams.All(workDto => workDto.Id != dto.Id))
            .Take(subscribesCount - work.Instagrams.Count).ToList();
        if (instagrams.Count == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет активированных аккаунтов.");
            return;
        }

        work.Instagrams.AddRange(instagrams);
        var result = await serviceContainer.WorkService.UpdateAsync(work);


        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Ошибка при добавлении аккаунтов: {result.ErrorMessage}.");
            return;
        }

        user.State = State.EnterMassage;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите сообщение, которое хотите разослать:", replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "selectAll" && user!.State == State.SelectAccounts;
    }
}