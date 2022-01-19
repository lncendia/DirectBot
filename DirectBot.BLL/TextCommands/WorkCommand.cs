using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class WorkCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите, что вы хотите сделать.", replyMarkup: WorkingKeyboard.Working);
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && message.Text == "❤ Задачи" && user!.State == State.Main;
    }
}