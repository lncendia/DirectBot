using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var id = long.Parse(query.Data![7..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User != user)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете добавить этот инстаграм.");
            return;
        }

        var work = new WorkDTO
        {
            Instagram = instagram
        };
        user.CurrentWorks.Add(work);
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите аккаунты:",
            replyMarkup: WorkingKeyboard.NewSelect(query.Message.ReplyMarkup!.InlineKeyboard.ToList()!, query.Data));
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data!.StartsWith("select") && user!.State == State.SelectAccounts;
    }
}