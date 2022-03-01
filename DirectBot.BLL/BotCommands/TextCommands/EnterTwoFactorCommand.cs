using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterTwoFactorCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var instagram = user!.CurrentInstagram == null
            ? null
            : await serviceContainer.InstagramService.GetAsync(user.CurrentInstagram.Id);
        if (instagram == null)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        await client.SendChatActionAsync(user.Id, ChatAction.Typing);
        var x = await serviceContainer.InstagramLoginService.EnterTwoFactorAsync(instagram, message.Text!);
        switch (x.Value)
        {
            case LoginTwoFactorResult.Success:
            {
                await serviceContainer.InstagramLoginService.SendRequestsAfterLoginAsync(instagram);
                instagram.IsActive = true;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                await client.SendTextMessageAsync(message.From!.Id,
                    "Инстаграм успешно активирован.");
                user.CurrentInstagram = null;
                user.State = State.Main;
                break;
            }
            case LoginTwoFactorResult.InvalidCode:
                await client.SendTextMessageAsync(message.From!.Id,
                    "Неверный код, попробуйте еще раз.", replyMarkup: MainKeyboard.Main);
                return;
            case LoginTwoFactorResult.CodeExpired:
                await client.SendTextMessageAsync(message.From!.Id,
                    "Время действия кода истекло, попробуйте еще раз.", replyMarkup: MainKeyboard.Main);
                return;
            case LoginTwoFactorResult.ChallengeRequired:
            {
                var challenge = await serviceContainer.InstagramLoginService.GetChallengeAsync(instagram);
                if (!challenge.Succeeded)
                {
                    await client.SendTextMessageAsync(message.From!.Id,
                        $"Ошибка: ({challenge.ErrorMessage}). Попробуйте войти ещё раз.");
                    user.State = State.Main;
                    user.CurrentInstagram = null;
                    break;
                }

                if (challenge.Value!.SubmitPhoneRequired)
                {
                    user.State = State.ChallengeRequiredPhoneCall;
                    await client.SendTextMessageAsync(message.From!.Id,
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
                await client.SendTextMessageAsync(message.From!.Id,
                    "Инстаграм просит подтверждение. Выберите, каким образом вы хотите получить код:",
                    replyMarkup: key);
            }
                break;
            case LoginTwoFactorResult.Exception:
            default:
                await client.SendTextMessageAsync(message.From!.Id,
                    $"Ошибка при отправке запроса ({x.ErrorMessage}). Попробуйте войти ещё раз.");
                user.State = State.Main;
                user.CurrentInstagram = null;
                break;
        }

        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.EnterTwoFactorCode;
    }
}