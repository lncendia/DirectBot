using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class BanCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"Вы были заблокированы. Для обжалования обратитесь в поддержку: {serviceContainer.Configuration.HelpAddress}.");
    }

    public bool Compare(Message message, UserDto? user)
    {
        return user!.IsBanned;
    }
}