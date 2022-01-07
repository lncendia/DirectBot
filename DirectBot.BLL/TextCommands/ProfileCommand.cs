using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class ProfileCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"<b>Ваш Id:</b> {user.Id}\n<b>Бонусный счет:</b> {user.Bonus}₽\n<b>Реферальная ссылка:</b> https://telegram.me/LikeChatVip_bot?start={user.Id}",
            ParseMode.Html, replyMarkup: Keyboards.Subscribes);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && message.Text == "🗒 Мой профиль" && user.State == State.Main;
    }
}