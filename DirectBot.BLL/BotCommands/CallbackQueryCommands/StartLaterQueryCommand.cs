using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class StartLaterQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        user!.State = State.SetDate;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(query.From.Id,
            "Через сколько вы хотите начать работу? В формате: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>", ParseMode.Html,
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "startLater" && user!.State == State.SelectTimeMode;
    }
}