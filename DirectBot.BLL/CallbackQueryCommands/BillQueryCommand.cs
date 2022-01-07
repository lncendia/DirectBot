using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class BillQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        if (new Payment().CheckPay(user, query.Data[5..], db))
        {
            string message = query.Message.Text;
            message = message.Replace("❌ Статус: Не оплачено", "✔ Статус: Оплачено");
            message = message.Remove(message.IndexOf("Оплачено", StringComparison.Ordinal) + 8);
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                message);
            await client.AnswerCallbackQueryAsync(query.Id, "Успешно оплачено.");
            if (user.Referal == null) return;
                
            db.UpdateRange(user, user.Referal);
            user.Referal.Bonus += BotSettings.Cfg.Bonus;
            try
            {
                await client.SendTextMessageAsync(user.Referal.Id,
                    $"По вашей реферальной ссылке перешел пользователь. Вам зачисленно {BotSettings.Cfg.Bonus} бонусных рублей.");
            }
            catch
            {
                //ignored
            }

            user.Referal = null;
                
            return;
        }

        await client.AnswerCallbackQueryAsync(query.Id, "Не оплачено.");
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("bill");
    }
}