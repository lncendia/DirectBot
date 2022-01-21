using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.BLL.Services;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterPhoneNumberCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var instagram = await serviceContainer.InstagramService.GetUserSelectedInstagramAsync(user!);
        if (instagram == null)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        await client.SendChatActionAsync(user!.Id, ChatAction.Typing);
        var result = await serviceContainer.InstagramLoginService.SubmitPhoneNumberAsync(instagram, message.Text!);
        if (result.Succeeded)
        {
            user!.State = State.ChallengeRequiredAccept;
            await serviceContainer.UserService.UpdateAsync(user);
            await client.SendTextMessageAsync(message.From!.Id,
                "Код отправлен. Введите код из сообщения.", replyMarkup: MainKeyboard.Main);
        }
        else
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Возможно введён неверный номер. Попробуйте ещё раз.", replyMarkup: MainKeyboard.Main);
        }
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.ChallengeRequiredPhoneCall;
    }
}