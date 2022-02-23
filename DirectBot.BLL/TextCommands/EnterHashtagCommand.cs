using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterHashtagCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var work = user!.CurrentWork;
        if (work == null)
        {
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id, "У вас нет активных задач. Вы в главном меню.");
            return;
        }


        var hashtag = message.Text!.Trim(' ');
        if (hashtag[0] == '#') hashtag = hashtag[1..];
        work.Hashtag = hashtag;
        user.State = State.SelectTypeWork;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.Chat.Id, "Введите тип работы:",
            replyMarkup: WorkingKeyboard.SelectTypeWork);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterHashtag;
    }
}