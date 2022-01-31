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
        var works = await serviceContainer.WorkService.GetUserActiveWorksAsync(user!);
        if (!works.Any())
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id,
                "У вас нет активных задач. Вы в главном меню.");
            return;
        }


        foreach (var work in works)
        {
            work.Hashtag = message.Text;
            await serviceContainer.WorkService.UpdateAsync(work);
        }

        user!.State = State.SelectTimeMode;
        await serviceContainer.UserService.UpdateAsync(user);


        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите действие:", replyMarkup: WorkingKeyboard.StartWork);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterHashtag;
    }
}