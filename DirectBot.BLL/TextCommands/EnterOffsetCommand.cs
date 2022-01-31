using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterOffsetCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var works = await serviceContainer.WorkService.GetUserActiveWorksAsync(user!);
        if (!works.Any())
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id,
                "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        var data = message.Text!.Split(':');
        if (data.Length != 2 || !int.TryParse(data[0], out int lower) || !int.TryParse(data[1], out int upper) ||
            lower > upper || lower < 0 || upper < 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[нижний предел:верхний предел]</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }

        if (upper > 300)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Интервал не может быть больше 5 минут!",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        foreach (var work in works)
        {
            work.LowerInterval = lower;
            work.UpperInterval = upper;
            await serviceContainer.WorkService.UpdateAsync(work);
        }


        user!.State = State.SelectType;
        await serviceContainer.UserService.UpdateAsync(user);


        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите тип:", replyMarkup: WorkingKeyboard.SelectType);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterOffset;
    }
}