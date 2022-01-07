using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class AcceptLogInQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        var login = await InstagramLoginService.ActivateAsync(user.CurrentInstagram);
        if (login.Succeeded && login.Value == InstaLoginResult.Success)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Успешно.");
        }
        else
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Произошла ошибка, попробуйте еще раз.");
        }
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "acceptEntry" && user.State == State.ChallengeRequired;
    }
}