using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.TextCommands;

public class AccountsCommand : ITextCommand
{
    private readonly Random _rnd = new();

    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        var instagrams = await serviceContainer.InstagramService.GetUserInstagramsAsync(user!);
        var subscribes = await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(user!);
        foreach (var x in instagrams)
        {
            int count = x.Password.Length / 2;
            var offsetLength = (x.Password.Length - count) / 2;

            string password = x.Password[..offsetLength] + new String('*', count) +
                              x.Password[(offsetLength + count)..];
            await client.SendTextMessageAsync(message.Chat.Id,
                $"Имя пользователя: <code>{x.Username}</code>\nПароль: <code>{password}</code>", ParseMode.Html,
                replyMarkup: InstagramLoginKeyboard.InstagramMain(x.Id, x.IsActive));
        }

        if (instagrams.Count < subscribes)
            await client.SendTextMessageAsync(message.Chat.Id,
                "Вы можете добавить аккаунт",
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("➕ Добавить аккаунт", "enterData")));
        else
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Оплатите подписку, чтобы добавить аккаунт.");
        }
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && message.Text == "🌇 Мои аккаунты" && user!.State == State.Main;
    }
}