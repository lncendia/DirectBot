using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterDurationCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        if (!message.Text.Contains(':') || !int.TryParse(message.Text.Split(':')[0], out var lowerDelay) ||
            !int.TryParse(message.Text.Split(':')[1], out var upperDelay))
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Неверный формат!", replyMarkup: Keyboards.Back("offset"));
            return;
        }

        if (upperDelay < lowerDelay)
        {
            await client.SendTextMessageAsync(message.From.Id,
                "Верхний предел не может быть больше нижнего!", replyMarkup: Keyboards.Back("offset"));
            return;
        }

        if (lowerDelay < BotSettings.Cfg.LoverDuration)
        {
            await client.SendTextMessageAsync(message.From.Id,
                $"Нижний предел не может быть меньше {BotSettings.Cfg.LoverDuration} секунд!",
                replyMarkup: Keyboards.Back("offset"));
            return;
        }

        if (upperDelay > BotSettings.Cfg.UpperDuration)
        {
            await client.SendTextMessageAsync(message.From.Id,
                $"Верхний предел не может быть больше {BotSettings.Cfg.UpperDuration} секунд!",
                replyMarkup: Keyboards.Back("offset"));
            return;
        }

        if (upperDelay - lowerDelay < BotSettings.Cfg.Interval)
        {
            await client.SendTextMessageAsync(message.From.Id,
                $"Интервал не может быть меньше {BotSettings.Cfg.Interval} секунд!",
                replyMarkup: Keyboards.Back("offset"));
            return;
        }

        user.CurrentWorks.ForEach(_ => _.SetDuration(lowerDelay, upperDelay));
        await client.SendTextMessageAsync(message.From.Id,
            "С какого поста начать отработку?", replyMarkup: Keyboards.SetOffset);
        user.State = State.SetOffset;
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.SetDuration;
    }
}