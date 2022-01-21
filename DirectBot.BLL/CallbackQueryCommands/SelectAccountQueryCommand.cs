using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var id = int.Parse(query.Data![7..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User!.Id != user!.Id || !instagram.IsActive)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Вы не можете добавить этот инстаграм.");
            return;
        }

        var work = new WorkDto
        {
            Instagram = instagram
        };
        var result = await serviceContainer.WorkService.AddAsync(work);
        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Ошибка: {result.ErrorMessage}.");
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите аккаунты:",
            replyMarkup: WorkingKeyboard.NewSelect(query.Message.ReplyMarkup!.InlineKeyboard.ToList()!, query.Data));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("select") && user!.State == State.SelectAccounts;
    }
}