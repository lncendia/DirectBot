using DirectBot.BLL.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MySubscribesQueryCommand:ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, CallbackQuery query, ServiceContainer serviceContainer)
    {
        string subscribes = $"У вас {user.Subscribes.Count} подписки(ок).\n";
        int i = 0;
        foreach (var sub in user.Subscribes.ToList())
        {
            i++;
            subscribes += $"Подписка {i}. Истекает {sub.EndSubscribe:D}\n";
        }
        await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, subscribes,
            replyMarkup: Keyboards.Back("subscribes"));
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "subscribes";
    }
}