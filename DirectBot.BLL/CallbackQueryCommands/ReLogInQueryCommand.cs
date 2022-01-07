using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ReLogInQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (user.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        Instagram inst = user.Instagrams.FirstOrDefault(_ => _.Id == int.Parse(query.Data[8..]));
        if (inst == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Инстаграм не найден.");
            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
            return;
        }

        if (inst.IsDeactivated)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Инстаграм деактивирован.");
            return;
        }

        //TODO: Check for active works;
        if (!await InstagramLoginService.DeactivateAsync(inst))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Не удалось выйти.");
            return;
        }

        await MainBot.Login(user, await InstagramLoginService.ActivateAsync(inst), db);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("reLogIn");
    }
}