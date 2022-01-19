using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartLaterQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        user!.State = State.SetDate;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(query.From.Id,
            "Через сколько вы хотите начать работу? В формате: <code>[ЧЧ:мм]</code>", ParseMode.Html,
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data == "startLater" && user!.State == State.SelectTimeMode;
    }
}