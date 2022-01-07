using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ExitQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (query.Data.StartsWith("exit"))
        {
            var instagram = user.Instagrams.FirstOrDefault(_ => _.Id == int.Parse(query.Data[5..]));
            if (instagram == null)
            {
                await client.AnswerCallbackQueryAsync(query.Id, "Инстаграм не найден.");
                await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
            }
            else
            {
                //TODO: Check for active works
                string message = (await InstagramLoginService.DeactivateAsync(instagram)) ? "Успешно." : "Не удалось выйти.";
                await client.AnswerCallbackQueryAsync(query.Id, message);
                await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
            }
        }
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("exit");
    }
}