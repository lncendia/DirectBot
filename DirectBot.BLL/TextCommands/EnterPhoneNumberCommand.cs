using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterPhoneNumberCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        var result = await InstagramLoginService.SubmitPhoneNumberAsync(user.CurrentInstagram, message.Text); 
        if (result.Succeeded)
        {
            user.State = State.ChallengeRequiredAccept;
            await client.SendTextMessageAsync(message.From.Id,
                "Код отправлен. Введите код из сообщения.", replyMarkup: Keyboards.Main);
        }
        else
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Ошибка. Возможно введён неверный номер. Попробуйте ещё раз.", replyMarkup: Keyboards.Main);
        }

    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.ChallengeRequiredPhoneCall;
    }
}