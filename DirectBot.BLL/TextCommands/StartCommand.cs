using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace DirectBot.BLL.TextCommands;

public class StartCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        user = new UserDto {Id = message.From!.Id, State = State.Main};
        var result = await serviceContainer.UserService.AddAsync(user);
        if (result.Succeeded)
        {
            var t1 = client.SendStickerAsync(message.From.Id,
                new InputOnlineFile("CAACAgIAAxkBAAEDh2ZhwNXpm0Vikt-5J5yPWTbDPeUwvwAC-BIAAkJOWUoAAXOIe2mqiM0jBA"));
            var t2 = client.SendTextMessageAsync(message.Chat.Id,
                "Здравствуйте!🙊\nЕсли хотите найти тот самый фильм из ТикТока😱\nПодпишись на каналы внизу ⬇ после нажми 🔍 Проверить\nИ переходи в канал с фильмом😉",
                replyMarkup: MainKeyboard.MainReplyKeyboard);
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