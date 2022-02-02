using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterCountCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var works = await serviceContainer.WorkService.GetUserActiveWorksAsync(user!);
        if (!works.Any())
        {
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id, "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        if (!int.TryParse(message.Text!, out var count) || count is < 1 or > 500)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Введены неверные данные. Количество получателей должно быть не менее 1 и не более 500. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        foreach (var work in works)
        {
            work.CountUsers = count;
            await serviceContainer.WorkService.UpdateAsync(work);
        }

        user!.State = State.SelectTimeMode;
        await serviceContainer.UserService.UpdateAsync(user);


        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите действие:", replyMarkup: WorkingKeyboard.StartWork);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterCountUsers;
    }
}