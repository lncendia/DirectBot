using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterDateCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        if (TimeSpan.TryParse(message.Text, out var timeSpan))
        {
            var timeEnter = DateTimeOffset.Now.Add(timeSpan.Duration());
            (await serviceContainer.WorkService.GetUserActiveWorksAsync(user!)).ForEach(
                work => work.JobId = serviceContainer.WorkerService.ScheduleWork(work, timeEnter));
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(user.Id, "Задачи успешно поставлены в очередь, вы в главном меню.");
        }
        else
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверный формат времени! Попробуйте ещё раз.\nФормат: <code>[ЧЧ:мм]</code>", ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
        }
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.SetDate;
    }
}