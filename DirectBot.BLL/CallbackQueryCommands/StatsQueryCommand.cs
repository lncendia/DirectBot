using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StatsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, CallbackQuery query, IUserService userService, 
        Core.Configuration.Configuration configuration)
    {
        var count = query.Data![6..] switch
        {
            "today" => userService.GetCount(DateTime.Now.Date, DateTime.Now, TimeZoneInfo.Local),
            "week" => userService.GetCount(DateTime.Now.Date.AddDays(-7), DateTime.Now, TimeZoneInfo.Local),
            "month" => userService.GetCount(DateTime.Now.Date.AddDays(-30), DateTime.Now, TimeZoneInfo.Local),
            "ever" => userService.GetCount(),
            _ => Result<int>.Fail("Неверный промежуток времени")
        };

        if (count.Succeeded)
        {
            await client.EditMessageTextAsync(user!.Id, query.Message!.MessageId,
                $"Уникальных пользователей: <code>{count.Value}</code>.", ParseMode.Html);
        }
        else
        {
            await client.EditMessageTextAsync(user!.Id, query.Message!.MessageId,
                $"Ошибка: {count.ErrorMessage}.");
        }
    }

    public bool Compare(CallbackQuery query, User? user)
    {
        return user!.State == State.Main && query.Data!.StartsWith("stats") && user.IsAdmin;
    }
}