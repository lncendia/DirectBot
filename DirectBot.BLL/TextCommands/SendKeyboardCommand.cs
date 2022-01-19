using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class SendKeyboardCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        user!.CurrentInstagram = null;
        foreach (var userCurrentWork in user.CurrentWorks.ToList()) //TODO: Remove ToList() if use DTOs, cause EF Remove entity after DeleteAsync
        {
            var result = await serviceContainer.WorkService.DeleteAsync(userCurrentWork);
            if (result.Succeeded) continue;
            await client.SendTextMessageAsync(message.From!.Id,
                $"Ошибка: {result.ErrorMessage}");
            return;
        }

        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.From!.Id,
            "Вы в главном меню.", replyMarkup: MainKeyboard.MainReplyKeyboard);
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && message.Text!.StartsWith("/start");
    }
}