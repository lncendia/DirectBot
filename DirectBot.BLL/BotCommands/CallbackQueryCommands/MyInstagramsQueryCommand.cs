using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Extensions;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

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

        var instagrams = await serviceContainer.InstagramService.GetUserInstagramsAsync(user.Id);
        if (instagrams.Any())
        {
            var instagram = instagrams.First();
            int count = instagram.Password.Length / 2;
            var offsetLength = (instagram.Password.Length - count) / 2;

            string password = (instagram.Password[..offsetLength] + new String('*', count) +
                              instagram.Password[(offsetLength + count)..]).ToHtmlStyle();
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Имя пользователя: <code>{instagram.Username.ToHtmlStyle()}</code>\nПароль: <code>{password}</code>", ParseMode.Html,
                replyMarkup: InstagramLoginKeyboard.InstagramMain(instagram));
        }


        for (int i = 1; i < instagrams.Count; i++)
        {
            var instagram = instagrams[i];
            int count = instagram.Password.Length / 2;
            var offsetLength = (instagram.Password.Length - count) / 2;

            string password = instagram.Password[..offsetLength] + new String('*', count) +
                              instagram.Password[(offsetLength + count)..];

            await client.SendTextMessageAsync(query.From.Id,
                $"Имя пользователя: <code>{instagram.Username.ToHtmlStyle()}</code>\nПароль: <code>{password}</code>", ParseMode.Html,
                replyMarkup: InstagramLoginKeyboard.InstagramMain(instagram));
        }
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "myInstagrams";
    }
}