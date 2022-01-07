using DirectBot.Core.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class SelectAllAccountsQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(TelegramBotClient client, User user, CallbackQuery query, Db db)
    {
        try
        {
            await client.DeleteMessageAsync(query.From.Id, query.Message.MessageId);
        }
        catch
        {
            // ignored
        }

        user.State = State.Block;
        foreach (var inst in user.Instagrams)
        {
            if (inst.IsDeactivated)
            {
                await client.SendTextMessageAsync(query.From.Id,
                    $"Аккаунт {inst.Username} деактивирован. Купите подписку, чтобы активировать аккаунт.");
                continue;
            }

            if (user.CurrentWorks.FirstOrDefault(x => x.Instagram.Username == inst.Username) != null)
            {
                await client.SendTextMessageAsync(query.From.Id,
                    $"Аккаунт {inst.Username} уже добавлен.");
                continue;
            }

            await client.SendTextMessageAsync(query.From.Id,
                $"Инстаграм {inst.Username} добавлен.");
            var work = new Work(user.Works.Count, inst, user);
            user.CurrentWorks.Add(work);
        }

        await client.SendTextMessageAsync(query.From.Id,
            "Выберите режим.", replyMarkup: Keyboards.SelectMode);
        user.State = State.SetMode;
    }

    public bool Compare(CallbackQuery query, User user)
    {
        return query.Data == "selectAll" && user.State == State.SelectAccounts;
    }
}