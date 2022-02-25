using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var id = int.Parse(query.Data![7..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        var work = user!.CurrentWork == null
            ? null
            : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);
        if (work == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Работа отсутсвтует.", replyMarkup: MainKeyboard.Main);
            return;
        }

        var subscribesCount =
            await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(user.Id);

        if (instagram == null || instagram.User!.Id != user.Id || !instagram.IsActive ||
            work.Instagrams.Count >= subscribesCount || work.Instagrams.Any(dto => dto.Id == instagram.Id))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете добавить этот инстаграм.");
            return;
        }

        work.Instagrams.Add(serviceContainer.Mapper.Map<InstagramLiteDto>(instagram));
        var result = await serviceContainer.WorkService.UpdateAsync(work);
        if (!result.Succeeded)
        {
            await client.AnswerCallbackQueryAsync(query.Id, $"Ошибка: {result.ErrorMessage}.");
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Выберите аккаунты:",
            replyMarkup: WorkingKeyboard.NewSelect(query.Message.ReplyMarkup!.InlineKeyboard.ToList()!, query.Data));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("select") && user!.State == State.SelectAccounts;
    }
}