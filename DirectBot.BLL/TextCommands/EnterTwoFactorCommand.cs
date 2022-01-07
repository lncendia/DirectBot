using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterTwoFactorCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        var x = await InstagramLoginService.EnterTwoFactorAsync(user.CurrentInstagram, message.Text);
        if (x is not { Succeeded: true })
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            return;
        }

        switch (x.Value)
        {
            case InstaLoginTwoFactorResult.Success:
            {
                await InstagramLoginService.SendRequestsAfterLoginAsync(user.CurrentInstagram);
                user.CurrentInstagram = null;
                await client.SendTextMessageAsync(message.From.Id,
                    "Инстаграм успешно добавлен.");
                user.State = State.Main;
                break;
            }
            case InstaLoginTwoFactorResult.InvalidCode:
                await client.SendTextMessageAsync(message.From.Id,
                    "Неверный код, попробуйте еще раз.", replyMarkup: Keyboards.Main);
                break;
            default:
                await client.SendTextMessageAsync(message.From.Id,
                    "Ошибка. Попробуйте войти ещё раз.");
                db.Remove(user.CurrentInstagram);
                user.State = State.Main;
                break;
        }
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.EnterTwoFactorCode;
    }
}