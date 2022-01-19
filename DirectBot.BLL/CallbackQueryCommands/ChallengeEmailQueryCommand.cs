using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ChallengeEmailQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.CurrentInstagram == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        var response =
            await serviceContainer.InstagramLoginService.EmailMethodChallengeRequiredAsync(user.CurrentInstagram);
        if (!response.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка ({response.ErrorMessage}). Попробуйте войти снова.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        user.State = State.ChallengeRequiredAccept;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Код отправлен. Введите код из сообщения.", replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data == "challengeEmail" && user!.State == State.ChallengeRequired;
    }
}