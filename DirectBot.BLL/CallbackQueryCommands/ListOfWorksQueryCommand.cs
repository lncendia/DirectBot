using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ListOfWorksQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, CallbackQuery query, ServiceContainer serviceContainer)
    {
        if (user.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        var works = db.Works.Where(work => work.Instagram.User == user);
        if (!works.Any())
        {
            await client.SendTextMessageAsync(query.From.Id,
                "У вас нет активных отработок.");
        }

        foreach (var work in works)
        {
            await client.SendTextMessageAsync(query.From.Id,
                $"Аккаунт {work.Instagram.Username}. Хештег #{work.Hashtag}. - {work.Progress:P}",
                replyMarkup: Keyboards.Cancel(work.Id));
        }
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "stopWorking";
    }
}