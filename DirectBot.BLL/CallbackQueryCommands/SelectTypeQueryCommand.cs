using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectTypeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var type = (WorkType) Enum.Parse(typeof(WorkType), query.Data![5..]);
        foreach (var dto in await serviceContainer.WorkService.GetUserActiveWorksAsync(user!))
        {
            dto.Type = type;
            await serviceContainer.WorkService.AddAsync(dto);
        }

        switch (type)
        {
            case WorkType.Subscriptions:
            case WorkType.Subscribers:
                user!.State = State.EnterCountUsers;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите число получателей. Должно быть не менее 1 и не более 500.",
                    replyMarkup: MainKeyboard.Main);
                break;
            case WorkType.Hashtag:
                user!.State = State.EnterHashtag;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите хештег:", replyMarkup: MainKeyboard.Main);
                break;
            case WorkType.File:
                user!.State = State.EnterFile;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Отправьте файл с Pk и/или Username пользователей в формате csv (первая ячейка - имя колонки):",
                    replyMarkup: MainKeyboard.Main);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("type") && user!.State == State.SelectType;
    }
}