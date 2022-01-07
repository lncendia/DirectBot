using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        user.State = State.Block;
        Instagram inst = user.Instagrams.Find(x => x.Id == int.Parse(query.Data[7..]));
        if (inst == null)
        {
            user.State = State.SelectAccounts;
            await client.AnswerCallbackQueryAsync(query.Id, "Инстаграм не найден.");
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                query.Message.Text,
                replyMarkup: Keyboards.NewSelect(
                    query.Message.ReplyMarkup.InlineKeyboard.ToList(),
                    query));
            return;
        }

        if (inst.IsDeactivated)
        {
            user.State = State.SelectAccounts;
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Аккаунт {inst.Username} деактивирован. Купите подписку, чтобы активировать аккаунт.");
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                query.Message.Text,
                replyMarkup: Keyboards.NewSelect(
                    query.Message.ReplyMarkup.InlineKeyboard.ToList(),
                    query));
            return;
        }

        if (user.CurrentWorks.FirstOrDefault(x => x.Instagram.Username == inst.Username) != null)
        {
            user.State = State.SelectAccounts;
            await client.AnswerCallbackQueryAsync(query.Id,
                $"Аккаунт {inst.Username} уже добавлен.");
            await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
                query.Message.Text,
                replyMarkup: Keyboards.NewSelect(
                    query.Message.ReplyMarkup.InlineKeyboard.ToList(),
                    query));
            return;
        }

        await client.AnswerCallbackQueryAsync(query.Id,
            $"Инстаграм {inst.Username} успешно добавлен.");
        await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId,
            query.Message.Text,
            replyMarkup: Keyboards.NewSelect(query.Message.ReplyMarkup.InlineKeyboard.ToList(),
                query));
        var work = new Work(user.Works.Count, inst, user);
        user.CurrentWorks.Add(work);
        user.State = State.SelectAccounts;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data.StartsWith("select_") && user.State == State.SelectAccounts;
    }
}