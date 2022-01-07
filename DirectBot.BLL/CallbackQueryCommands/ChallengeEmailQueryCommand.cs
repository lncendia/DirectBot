using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ChallengeEmailQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        try
        {
            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        }
        catch
        {
            //ignored
        }

        var response = await InstagramLoginService.EmailMethodChallengeRequiredAsync(user.CurrentInstagram);
        if (!response.Succeeded)
        {
            await client.SendTextMessageAsync(query.From.Id,
                "Ошибка. Попробуйте войти снова.");
            user.State = State.Main;
            return;
        }

        user.State = State.ChallengeRequiredAccept;
        await client.SendTextMessageAsync(query.From.Id,
            "Код отправлен. Введите код из сообщения.", replyMarkup: Keyboards.Main);
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "challengeEmail" && user.State == State.ChallengeRequired;
    }
}