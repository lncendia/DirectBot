using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace DirectBot.BLL.TextCommands;

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