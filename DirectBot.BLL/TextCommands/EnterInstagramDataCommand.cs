using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterInstagramDataCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var data = message.Text!.Split(':');
        if (data.Length != 2)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[логин:пароль]</code>", ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
            return;
        }

        var instagram = new InstagramDto
        {
            Username = data[0],
            Password = data[1],
            User = new UserLiteDto {Id = user!.Id}
        };
        var result = await serviceContainer.InstagramService.AddAsync(instagram);
        if (result.Succeeded)
        {
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id,
                "Инстаграм успешно добавлен.", replyMarkup: InstagramLoginKeyboard.Activate(instagram.Id));
            return;
        }

        await client.SendTextMessageAsync(message.Chat.Id,
            $"Ошибка: {result.ErrorMessage}. Попробуйте ещё раз.",
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterInstagramData;
    }
}