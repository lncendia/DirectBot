using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class StartEnterAccountDataQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (user.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        await client.DeleteMessageAsync(query.From.Id,
            query.Message.MessageId);
        if (user.Instagrams.Count >= user.Subscribes.Count)
        {
            await client.SendTextMessageAsync(query.From.Id,
                "Увы... Так не работает.");
            return;
        }

        user.CurrentInstagram = new Instagram { User = user };
        user.State = State.EnterLogin;
        await client.SendTextMessageAsync(query.From.Id,
            "Введите логин", replyMarkup: Keyboards.Main);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "enterData";
    }
}