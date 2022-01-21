using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MyInstagramsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![13..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var instagram = await serviceContainer.InstagramService.GetUserInstagramsAsync(user, page);
        if (instagram == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет аккаунтов.");
            return;
        }


        int count = instagram.Password.Length / 2;
        var offsetLength = (instagram.Password.Length - count) / 2;

        string password = instagram.Password[..offsetLength] + new String('*', count) +
                          instagram.Password[(offsetLength + count)..];

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            $"Имя пользователя: <code>{instagram.Username}</code>\nПароль: <code>{password}</code>", ParseMode.Html,
            replyMarkup: InstagramLoginKeyboard.InstagramMain(page, instagram));
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("myInstagrams");
    }
}