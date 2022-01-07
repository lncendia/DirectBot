using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.Interfaces;

public interface ITextCommand
{
    public Task Execute(ITelegramBotClient client, User? user, Message message, IUserService userService, 
        Core.Configuration.Configuration configuration);

    public bool Compare(Message message, User? user);
}