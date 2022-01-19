using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class HelpCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.Chat.Id,
            "За поддержкой вы можете обратиться к @Per4at."); //TODO:Config
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && message.Text == "🤝 Поддержка";
    }
}