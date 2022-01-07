using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.Interfaces;

public interface ICallbackQueryCommand
{
    public Task Execute(ITelegramBotClient client, User? user, CallbackQuery query, IUserService userService,
         Core.Configuration.Configuration configuration);

    public bool Compare(CallbackQuery query, User? user);
}