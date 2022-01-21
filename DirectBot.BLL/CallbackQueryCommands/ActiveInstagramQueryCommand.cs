using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.CallbackQueryCommands;

public class ActiveInstagramQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![7..]);
        var instagram = await serviceContainer.InstagramService.GetAsync(id);
        if (instagram == null || instagram.User!.Id != user!.Id)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете активировать этот инстаграм.");
            return;
        }

        await client.SendChatActionAsync(user!.Id, ChatAction.Typing);
        var result = await serviceContainer.InstagramLoginService.ActivateAsync(instagram);
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
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Неверный пароль.",
                    replyMarkup: InstagramLoginKeyboard.Edit(instagram.Id));
                return;
            case LoginResult.InvalidUser:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Пользователь не найден.", replyMarkup: InstagramLoginKeyboard.Edit(instagram.Id));
                return;
            case LoginResult.InactiveUser:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Пользователь не активен.", replyMarkup: InstagramLoginKeyboard.Edit(instagram.Id));
                return;
            case LoginResult.LimitError:
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Слишком много запросов. Подождите несколько минут и попробуйте снова.");
                return;
            case LoginResult.TwoFactorRequired:
                await client.SendTextMessageAsync(query.From.Id,
                    "Необходима двухфакторная аутентификация. Введите проверочный код.",
                    replyMarkup: MainKeyboard.Main);
                instagram.IsSelected = true;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                user!.State = State.EnterTwoFactorCode;
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
                    user!.State = State.ChallengeRequiredPhoneCall;
                    await client.SendTextMessageAsync(query.From.Id,
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

                user!.State = State.ChallengeRequired;
                instagram.IsSelected = true;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                await client.SendTextMessageAsync(query.From.Id,
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

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("active");
    }
}