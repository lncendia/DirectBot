using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class SelectUsersTypeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var type = (WorkUsersType) Enum.Parse(typeof(WorkUsersType), query.Data![10..]);
        if (user!.CurrentWork == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Работа не выбрана. Попробуйте ещё раз.");
            return;
        }

        user.CurrentWork.UsersType = type;
        var count = await serviceContainer.WorkService.GetInstagramsCountAsync(user.CurrentWork.Id);

        switch (type)
        {
            case WorkUsersType.Subscriptions:
            case WorkUsersType.Subscribers:
                if (count > 1)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        "Работы такого типа можно запустить только с одним инстаграмом.");
                    return;
                }

                user.State = State.SelectTypeWork;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите тип работы:", replyMarkup: WorkingKeyboard.SelectTypeWork);
                break;
            case WorkUsersType.Hashtag:
                user.State = State.EnterHashtag;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите хештег:", replyMarkup: MainKeyboard.Main);
                break;
            case WorkUsersType.File:
                user.State = State.EnterFile;
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
        return query.Data!.StartsWith("usersType") && user!.State == State.SelectTypeUsers;
    }
}