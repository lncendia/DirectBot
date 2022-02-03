using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterFileCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var work = await serviceContainer.WorkService.GetUserSelectedWorkAsync(user!);
        if (work == null)
        {
            user!.State = State.Main;
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
        await serviceContainer.WorkService.UpdateAsync(work);


        user!.State = State.EnterCountUsers;
        await serviceContainer.UserService.UpdateAsync(user);


        await client.SendTextMessageAsync(message.Chat.Id,
            "Введите число получателей. Должно быть не менее 1 и не более 500.", replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Document && user!.State == State.EnterFile;
    }
}