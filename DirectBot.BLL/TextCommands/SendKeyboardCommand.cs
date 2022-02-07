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
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var work = user!.CurrentWork == null
            ? null
            : await serviceContainer.WorkService.GetAsync(user.CurrentWork.Id);
        if (work != null)
        {
            var result = await serviceContainer.WorkService.DeleteAsync(work);
            if (!result.Succeeded)
            {
                await client.SendTextMessageAsync(message.From!.Id, $"Ошибка: {result.ErrorMessage}",
                    replyMarkup: MainKeyboard.Main);
                return;
            }
        }

        user.CurrentInstagram = null;
        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.SendTextMessageAsync(message.From!.Id,
            "Вы в главном меню.", replyMarkup: MainKeyboard.MainReplyKeyboard);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && message.Text!.StartsWith("/start");
    }
}