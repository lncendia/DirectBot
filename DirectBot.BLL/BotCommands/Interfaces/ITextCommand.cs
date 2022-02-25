using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.Interfaces;

public interface ITextCommand
{
    public Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer);

    public bool Compare(Message message, UserDto? user);
}