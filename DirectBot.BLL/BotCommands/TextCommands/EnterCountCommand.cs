using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterCountCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var work = user!.CurrentWork;
        if (work == null)
        {
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.Chat.Id, "У вас нет активных задач. Вы в главном меню.");
            return;
        }

        if (!int.TryParse(message.Text!, out var count) || count < 1)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Введены неверные данные. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        switch (work.Type)
        {
            case WorkType.Simple when count > 500:
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Количество пользователей для данного типа работ должно быть не больше 500. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
            case WorkType.Divide when count > 1500:
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Количество пользователей для данного типа работ должно быть не больше 1500. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
            case WorkType.Divide when count / work.CountPerDivision < 2:
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Количество пользователей в подзадачах должно быть меньше не менее, чем в два раза. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
        }

        work.CountUsers = count;
        user.State = State.SelectTimeMode;
        await serviceContainer.UserService.UpdateAsync(user);


        await client.SendTextMessageAsync(message.Chat.Id,
            "Выберите действие:", replyMarkup: WorkingKeyboard.StartWork);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterCountUsers;
    }
}