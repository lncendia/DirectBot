using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class InstructionCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"Всю инструкцию вы можете прочитать в канале {serviceContainer.Configuration.InstructionAddress}.");
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text == "📄 Инструкция";
    }
}