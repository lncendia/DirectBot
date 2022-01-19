using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ReLogInQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = long.Parse(query.Data![5..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User != user)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете выйти из этого инстаграма.");
            return;
        }

        if (await serviceContainer.WorkService.HasActiveWorksAsync(instagram))
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "На этом аккаунте есть незавершенные задачи.");
            return;
        }

        await serviceContainer.InstagramLoginService.DeactivateAsync(instagram);
        instagram.IsActive = false;
        instagram.StateData = null;
        instagram.TwoFactorLoginInfo = null;
        instagram.ChallengeLoginInfo = null;
        await serviceContainer.InstagramService.UpdateAsync(instagram);
        
        
        var result = await serviceContainer.InstagramLoginService.ActivateAsync(instagram);
        if (!result.Succeeded)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Ну удалось активировать инстаграм: {result.ErrorMessage}.");
            return;
        }

        switch (result.Value)
        {
            case LoginResult.Success:
            {
                await serviceContainer.InstagramLoginService.SendRequestsAfterLoginAsync(instagram);
                instagram.IsActive = true;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Инстаграм успешно активирован.");
                break;
            }
            case LoginResult.BadPassword:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Неверный пароль.");
                return;
            case LoginResult.InvalidUser:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Пользователь не найден.");
                return;
            case LoginResult.InactiveUser:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Пользователь не активен.");
                return;
            case LoginResult.LimitError:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Слишком много запросов. Подождите несколько минут и попробуйте снова.");
                return;
            case LoginResult.TwoFactorRequired:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Необходима двухфакторная аутентификация. Введите код из сообщения.",
                    replyMarkup: MainKeyboard.Main);
                user.State = State.EnterTwoFactorCode;
                break;
            case LoginResult.ChallengeRequired:
            {
                var challenge = await serviceContainer.InstagramLoginService.GetChallengeAsync(instagram);
                if (!challenge.Succeeded)
                {
                    await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                        $"Ошибка ({challenge.ErrorMessage}). Попробуйте войти ещё раз.");
                    return;
                }

                if (challenge.Value!.SubmitPhoneRequired)
                {
                    user.State = State.ChallengeRequiredPhoneCall;
                    await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                        "Инстаграм просит подтверждение. Введите подключенный к аккаунту номер.",
                        replyMarkup: MainKeyboard.Main);
                    break;
                }

                InlineKeyboardMarkup key;
                if (string.IsNullOrEmpty(challenge.Value.PhoneNumber))
                {
                    key = InstagramLoginKeyboard.Email(challenge.Value.Email!);
                }
                else if (string.IsNullOrEmpty(challenge.Value.Email))
                {
                    key = InstagramLoginKeyboard.Phone(challenge.Value.PhoneNumber);
                }
                else
                {
                    key = InstagramLoginKeyboard.PhoneAndEmail(challenge.Value.Email,
                        challenge.Value.PhoneNumber);
                }

                user.State = State.ChallengeRequired;
                user.CurrentInstagram = instagram;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Инстаграм просит подтверждение. Выбирете, каким образом вы хотите получить код:",
                    replyMarkup: key);
            }
                break;
            case LoginResult.Exception:
            case LoginResult.CheckpointLoggedOut:
            default:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Ошибка при отправке запроса. Попробуйте войти ещё раз!");
                return;
        }

        await serviceContainer.UserService.UpdateAsync(user);
        
    }

    public bool Compare(CallbackQuery query, UserDTO? user)
    {
        return query.Data!.StartsWith("reLogIn");
    }
}