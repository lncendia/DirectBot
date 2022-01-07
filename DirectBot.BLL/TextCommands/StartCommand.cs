using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class StartCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        user = new User {Id = message.From.Id, State = State.Main};
        if (message.Text.Length > 7 && long.TryParse(message.Text[7..], out long id))
        {
            User referal = db.Users.FirstOrDefault(user1 => user1.Id == id);
            if (referal != null)
            {
                user.Referal = referal;
            }
        }
        db.Add(user);
        await client.SendStickerAsync(message.From.Id,
            new InputOnlineFile("CAACAgIAAxkBAAK_HGAQINBHw7QKWWRV4LsEU4nNBxQ3AAKZAAPZvGoabgceWN53_gIeBA"),
            replyMarkup: Keyboards.MainKeyboard);
        await client.SendTextMessageAsync(message.Chat.Id,
            "Добро пожаловать.\nДля дальнейшей работы тебе необходимо оплатить подписку и ввести данные своего instagram.");
    }

    public bool Compare(Message message, User user)
    {
        return user is null;
    }
}