using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class MainMenuQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        await client.DeleteMessageAsync(query.From.Id,
            query.Message.MessageId);
        db.RemoveRange(user.CurrentInstagram, user.CurrentWorks);
        user.State = State.Main;
        await client.SendTextMessageAsync(query.From.Id,
            "Вы в главном меню.", replyMarkup: Keyboards.MainKeyboard);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "mainMenu" && user.State != State.Block;
    }
}