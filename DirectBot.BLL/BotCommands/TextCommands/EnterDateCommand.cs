using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterDateCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        if (TimeSpan.TryParse(message.Text, out var timeSpan))
        {
            var timeEnter = DateTimeOffset.Now.Add(timeSpan.Duration());
            var work = user!.CurrentWork == null
                ? null
                : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);

            if (work == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Ошибка. Работа отсутсвтует.", replyMarkup: MainKeyboard.Main);
                return;
            }

            var result = await serviceContainer.WorkerService.ScheduleWorkAsync(work, timeEnter);
            if (!result.Succeeded)
            {
                await client.SendTextMessageAsync(message.From!.Id,
                    $"Ошибка при запуске работы: {result.ErrorMessage}.", replyMarkup: MainKeyboard.Main);
                return;
            }

            user.CurrentWork = null;
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(user.Id, "Задачи успешно поставлены в очередь, вы в главном меню.");
        }
        else
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверный формат времени! Попробуйте ещё раз.\nФормат: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>",
                ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
        }
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.SetDate;
    }
}