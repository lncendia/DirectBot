using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.AdminKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class AdminStatsCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, Message message,
        IUserService userService,
        Core.Configuration.Configuration configuration)
    {
        await client.SendTextMessageAsync(user!.Id,
            "Выберите период времени:", replyMarkup: StatsKeyboard.Stats);
    }

    public bool Compare(Message message, User? user)
    {
        return message.Type == MessageType.Text && message.Text == "/stats" &&
               user!.State is State.Main && user.IsAdmin;
    }
}