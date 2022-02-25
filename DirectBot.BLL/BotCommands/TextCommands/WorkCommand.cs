using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class WorkCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите, что вы хотите сделать.", replyMarkup: WorkingKeyboard.Working);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text == "❤ Задачи" && user!.State == State.Main;
    }
}