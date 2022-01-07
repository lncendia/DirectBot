using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.TextCommands;

public class EnterChallengeRequireCodeCommand : ITextCommand
{
    public async Task Execute(TelegramBotClient client, User user, Message message, Db db)
    {
        var result = await InstagramLoginService.SubmitChallengeAsync(user.CurrentInstagram, message.Text);
        await MainBot.Login(user, result, db);
    }

    public bool Compare(Message message, User user)
    {
        return message.Type == MessageType.Text && user.State == State.ChallengeRequiredAccept;
    }
}