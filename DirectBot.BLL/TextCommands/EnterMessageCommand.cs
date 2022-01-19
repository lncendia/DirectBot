using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterMessageCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        if (!user!.CurrentWorks.Any())
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id,
                "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        user.CurrentWorks.ForEach(work => work.Message = message.Text);
        user.State = State.EnterOffset;
        await serviceContainer.UserService.UpdateAsync(user);

        await client.SendTextMessageAsync(message.Chat.Id,
            "Введите интервал отправки сообщений в секундах в формате: <code>[нижний предел:верхний предел]</code>",
            ParseMode.Html, replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterMassage;
    }
}