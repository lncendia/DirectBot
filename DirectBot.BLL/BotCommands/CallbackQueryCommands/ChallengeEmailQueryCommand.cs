using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class ChallengeEmailQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var instagram = user!.CurrentInstagram == null
            ? null
            : await serviceContainer.InstagramService.GetAsync(user.CurrentInstagram.Id);
        if (instagram == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        var response =
            await serviceContainer.InstagramLoginService.EmailMethodChallengeRequiredAsync(instagram);
        if (!response.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ошибка ({response.ErrorMessage}). Попробуйте войти снова.");
            user!.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        user.State = State.ChallengeRequiredAccept;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Код отправлен. Введите код из сообщения.", replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "challengeEmail" && user!.State == State.ChallengeRequired;
    }
}