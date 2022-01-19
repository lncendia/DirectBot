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
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        if (TimeSpan.TryParse(message.Text, out var timeSpan))
        {
            var timeEnter = DateTimeOffset.Now.Add(timeSpan.Duration());
            user!.CurrentWorks.ForEach(
                work => work.JobId = serviceContainer.WorkerService.ScheduleWork(work, timeEnter));
            user.State = State.Main;
            user.CurrentWorks.Clear();
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

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && user!.State == State.SetDate;
    }
}