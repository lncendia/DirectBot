using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterEditInstagramDataCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var instagram = user!.CurrentInstagram == null
            ? null
            : await serviceContainer.InstagramService.GetAsync(user.CurrentInstagram.Id);
        if (instagram == null)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        var data = message.Text!.Split(':');
        if (data.Length != 2)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[логин:пароль]</code>", ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
            return;
        }

        instagram.Username = data[0];
        instagram.Password = data[1];
        instagram.IsActive = false;
        await serviceContainer.InstagramService.UpdateAsync(instagram);
        user.CurrentInstagram = null;
        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.Chat.Id,
            "Инстаграм успешно изменён.", replyMarkup: InstagramLoginKeyboard.Activate(instagram.Id));
        return;
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterEditInstagramData;
    }
}