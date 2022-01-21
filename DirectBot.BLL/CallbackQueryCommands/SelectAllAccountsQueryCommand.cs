using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAllAccountsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var instagrams = await serviceContainer.InstagramService.GetUserActiveInstagramsAsync(user!);
        if (instagrams.Count == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет активированных аккаунтов.");
            return;
        }

        var works = instagrams.Select(instagram => new WorkDto
        {
            Instagram = instagram,
        });
        var tasks = works.Select(dto => serviceContainer.WorkService.AddAsync(dto));
        await Task.WhenAll(tasks);
        user!.State = State.EnterMassage;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите сообщение, которое хотите разослать:",
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "selectAll" && user!.State == State.SelectAccounts;
    }
}