using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.Interfaces;

public interface ITextCommand
{
    public Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer);

    public bool Compare(Message message, UserDTO? user);
}