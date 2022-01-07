using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.AdminKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class AdminInfoCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, Message message,
        IUserService userService, 
        Core.Configuration.Configuration configuration)
    {
        var tasks = channelService.GetAll().Select(channel => client.GetChatAsync(new ChatId(channel.Id)));
        try
        {
            var result = await Task.WhenAll(tasks);
            await client.SendTextMessageAsync(user!.Id,
                "Выберите канал:", replyMarkup: InfoKeyboard.Info(result.Select(chat=>(chat.Username, chat.Id)).ToList()));
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(user!.Id,
                $"Не удалось получить названия каналов: <code>{ex.Message}</code>.", ParseMode.Html);
        }
    }

    public bool Compare(Message message, User? user)
    {
        return message.Type == MessageType.Text && message.Text == "/info" &&
               user!.State is State.Main && user.IsAdmin;
    }
}