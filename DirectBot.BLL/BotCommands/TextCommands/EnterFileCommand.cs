using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterFileCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        var work = user!.CurrentWork;
        if (work == null)
        {
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id, "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        if (message.Document!.FileSize > 2097152)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Размер файла не должен превышать 2 Мб. Попробуйте ещё раз.", replyMarkup: MainKeyboard.Main);
            return;
        }

        work.FileIdentifier = message.Document!.FileId;
        user.State = State.SelectTypeWork;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.Chat.Id, "Введите тип работы:",
            replyMarkup: WorkingKeyboard.SelectTypeWork);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Document && user!.State == State.EnterFile;
    }
}