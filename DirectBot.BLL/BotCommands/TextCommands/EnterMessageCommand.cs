using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterMessageCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var work = user!.CurrentWork;
        if (work == null)
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id, "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        work.Message = message.Text;
        user.State = State.EnterOffset;
        await serviceContainer.UserService.UpdateAsync(user);

        await client.SendTextMessageAsync(message.Chat.Id,
            "Введите интервал отправки сообщений в секундах в формате: <code>[нижний предел:верхний предел]</code>",
            ParseMode.Html, replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterMassage;
    }
}