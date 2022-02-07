using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterSubscribeDataCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        string[] data = message.Text!.Split(' ', 2);

        if (!long.TryParse(data[0], out long id))
        {
            await client.SendTextMessageAsync(user!.Id, "Неверный Id. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        DateTime date;
        if (data[1] == "s") date = DateTime.UtcNow.AddDays(30);
        else
        {
            if (!DateTime.TryParse(data[1], out date) || date.CompareTo(DateTime.UtcNow) <= 0)
            {
                await client.SendTextMessageAsync(user!.Id, "Неверно введена дата. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
            }
        }

        var user2 = await serviceContainer.UserService.GetAsync(id);

        if (user2 == null)
        {
            await client.SendTextMessageAsync(user!.Id, "Пользователь не найден. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        var result = await serviceContainer.SubscribeService.AddAsync(new SubscribeDto
        {
            User = user,
            EndSubscribe = date
        });

        if (result.Succeeded)
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(user.Id, "Успешно. Вы в главном меню.");
            await client.SendTextMessageAsync(user2.Id, $"Администратор активировал вам подписку до {date:D}");
        }
        else
        {
            await client.SendTextMessageAsync(user!.Id, $"Не удалось добавить подписку: {result.ErrorMessage}.");
        }
    }


    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.SubscribesAdmin && user.IsAdmin;
    }
}