using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterHashtagCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (!message.Text.All(_ => char.IsLetterOrDigit(_) || "_".Contains(_)))
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Хештег может содержать только буквы и цифры! Введите хештег заново.", replyMarkup: Keyboards.Back("selectMode"));
            return;
        }

        user.CurrentWorks.ForEach(_ => _.SetHashtag(message.Text));
        user.State = State.SetDuration;
        await client.SendTextMessageAsync(message.From.Id,
            "Введите пределы интервала в секундах. (<strong>Пример:</strong> <em>30:120</em>).\nРекомендуемые параметры нижнего предела:\nНоввый аккаунт: <code>120 секунд.</code>\n3 - 6 месяцев: <code>90 секунд.</code>\nБольше года: <code>72 секунды.</code>\n",
            replyMarkup: Keyboards.Back("interval"), parseMode: ParseMode.Html);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.SetHashtag;
    }
}