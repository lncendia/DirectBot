using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterChallengeRequireCodeCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDTO? user, Message message, ServiceContainer serviceContainer)
    {
        if (user!.CurrentInstagram == null)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Ошибка. Попробуйте войти ещё раз.");
            user.State = State.Main;
            await serviceContainer.UserService.UpdateAsync(user);
            return;
        }

        var x = await serviceContainer.InstagramLoginService.SubmitChallengeAsync(user.CurrentInstagram, message.Text!);
        switch (x.Value)
        {
            case LoginResult.Success:
                await serviceContainer.InstagramLoginService.SendRequestsAfterLoginAsync(user.CurrentInstagram);
                user.CurrentInstagram.IsActive = true;
                await serviceContainer.InstagramService.UpdateAsync(user.CurrentInstagram);
                await client.SendTextMessageAsync(message.From!.Id,
                    "Инстаграм успешно активирован.");
                user.State = State.Main;
                user.CurrentInstagram = null;
                break;
            case LoginResult.ChallengeRequired:
                await client.SendTextMessageAsync(message.From!.Id,
                    $"Ошибка: ({x.ErrorMessage}). Проверьте код и введите его ещё раз.", replyMarkup: MainKeyboard.Main);
                break;
            case LoginResult.LimitError:
            case LoginResult.Exception:
            case LoginResult.BadPassword:
            case LoginResult.InvalidUser:
            case LoginResult.TwoFactorRequired:
            case LoginResult.InactiveUser:
            case LoginResult.CheckpointLoggedOut:
            default:
                await client.SendTextMessageAsync(message.From!.Id,
                    $"Ошибка при отправке запроса: ({x.Value}). Попробуйте войти ещё раз.");
                user.State = State.Main;
                user.CurrentInstagram = null;
                break;
        }

        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDTO? user)
    {
        return message.Type == MessageType.Text && user!.State == State.ChallengeRequiredAccept;
    }
}