using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class GoBackQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        switch (query.Data[5..])
        {
            case "password":
                if (user.State != State.EnterPassword)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не вводите пароль сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                user.State = State.EnterLogin;
                await client.DeleteMessageAsync(query.From.Id,
                    query.Message.MessageId);
                await client.SendTextMessageAsync(query.From.Id,
                    "Введите логин", replyMarkup: Keyboards.Main);
                break;
            case "selectMode":
                if (user.State != State.SetHashtag)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не вводите хештег сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                user.State = State.SetMode;
                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                    "Выберите режим.", replyMarkup: Keyboards.SelectMode);
                break;
            case "interval":
                if (user.State != State.SetDuration)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не вводите интервал сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                user.State = State.SetHashtag;
                await client.DeleteMessageAsync(query.From.Id,
                    query.Message.MessageId);
                await client.SendTextMessageAsync(query.From.Id,
                    "Введите хештег без #.", replyMarkup: Keyboards.Back("selectMode"));
                break;
            case "date":
                if (user.State != State.SetDate)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не указываете дату сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                await client.SendTextMessageAsync(query.From.Id,
                    "Выбирете, когда хотите начать.", replyMarkup: Keyboards.StartWork);
                await client.DeleteMessageAsync(query.From.Id,
                    query.Message.MessageId);
                user.State = State.SetTimeWork;
                break;
            case "offset":
                if (user.State != State.SetOffset && user.State != State.SetDuration)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не указываете сдвиг сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                user.State = State.SetDuration;
                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                    "Введите пределы интервала в секундах. (<strong>Пример:</strong> <em>30:120</em>).\nРекомендуемые параметры нижнего предела:\nНоввый аккаунт: <code>120 секунд.</code>\n3 - 6 месяцев: <code>90 секунд.</code>\nБольше года: <code>72 секунды.</code>\n",
                    replyMarkup: Keyboards.Back("interval"), parseMode: ParseMode.Html);
                break;
            case "offsetSelect":
                if (user.State != State.EnterOffset)
                {
                    await client.AnswerCallbackQueryAsync(query.Id,
                        $"Вы не вводите сдвиг сейчас.");
                    await client.DeleteMessageAsync(query.From.Id,
                        query.Message.MessageId);
                    break;
                }

                await client.DeleteMessageAsync(query.From.Id,
                    query.Message.MessageId);
                await client.SendTextMessageAsync(query.From.Id,
                    "С какого поста начать отработку?", replyMarkup: Keyboards.SetOffset);
                user.State = State.SetOffset;
                break;
            case "subscribes":
                await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                    $"<b>Ваш Id:</b> {user.Id}\n<b>Бонусный счет:</b> {user.Bonus}₽\n<b>Реферальная ссылка:</b> https://telegram.me/LikeChatVip_bot?start={user.Id}",
                    ParseMode.Html, replyMarkup: Keyboards.Subscribes);
                break;
        }
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("back");
    }
}