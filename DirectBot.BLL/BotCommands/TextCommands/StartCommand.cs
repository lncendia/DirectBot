using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class StartCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        user = new UserDto {Id = message.From!.Id, State = State.Main};
        var result = await serviceContainer.UserService.AddAsync(user);
        if (result.Succeeded)
        {
            var t1 = client.SendStickerAsync(message.From.Id,
                new InputOnlineFile("CAACAgIAAxkBAAEDh2ZhwNXpm0Vikt-5J5yPWTbDPeUwvwAC-BIAAkJOWUoAAXOIe2mqiM0jBA"),
                replyMarkup: MainKeyboard.MainReplyKeyboard);
            var t2 = client.SendTextMessageAsync(message.Chat.Id,
                "<b>Здравствуйте!</b>\n\nDirect рассылка - это бот для рассылки сообщений в директ Инстаграм.\n\nС помощь него Вы можете делать рассылку по хештегу, по подписчикам, по подпискам и по целевой аудитории через файл!",
                ParseMode.Html, replyMarkup: PaymentKeyboard.PaySubscribe);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                $"Произошла ошибка ({result.ErrorMessage}). Обратитесь в поддержку.");
        }
    }

    public bool Compare(Message message, UserDto? user)
    {
        return user is null;
    }
}