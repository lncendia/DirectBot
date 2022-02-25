using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterDivideDataCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var data = message.Text!.Split('-', 2);
        if (data.Length != 2)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверный формат данных Попробуйте ещё раз.\nФормат: <code>[число пользователей в подзадачах-интервал между подзадачами]</code>\nФормат даты: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>\nПример: <code>50-12:00</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }

        if (!int.TryParse(data[0], out var count) || count < 1)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверное число пользователей! Попробуйте ещё раз.\nФормат: <code>[число пользователей в подзадачах-интервал между подзадачами]</code>\nФормат даты: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>\nПример: <code>50-12:00</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }

        if (!TimeSpan.TryParse(data[1], out var timeSpan))
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверный интервал! Попробуйте ещё раз.\nФормат: <code>[число пользователей в подзадачах-интервал между подзадачами]</code>\nФормат даты: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>\nПример: <code>50-12:00</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }


        var work = user!.CurrentWork;
        if (work == null)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Ошибка. Работа отсутсвтует.", replyMarkup: MainKeyboard.Main);
            return;
        }

        work.CountPerDivision = count;
        work.IntervalPerDivision = timeSpan;
        user.State = State.EnterCountUsers;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(user.Id,
            "Введите число получателей. Должно быть не менее 1 и не более 500 для обычных и 1500 для разделенных работ.",
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterDivideData;
    }
}