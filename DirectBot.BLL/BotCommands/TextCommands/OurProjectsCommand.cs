using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class OurProjectsCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id, "Наши проекты:",
            replyMarkup: MainKeyboard.Projects(serviceContainer.Configuration.Projects));
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text == "📲 Наши проекты";
    }
}