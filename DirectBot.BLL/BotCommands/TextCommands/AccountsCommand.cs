using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class AccountsCommand : ITextCommand
{
    private readonly Random _rnd = new();

    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите, что вы хотите сделать.", replyMarkup: InstagramLoginKeyboard.MyAccounts);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text == "🌇 Мои аккаунты" && user!.State == State.Main;
    }
}