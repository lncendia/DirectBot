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
        var x = await serviceContainer.InstagramLoginService.SubmitChallengeAsync(instagram, message.Text!);
        switch (x.Value)
        {
            case LoginResult.Success:
                await serviceContainer.InstagramLoginService.SendRequestsAfterLoginAsync(instagram);
                instagram.IsActive = true;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                await client.SendTextMessageAsync(message.From!.Id,
                    "Инстаграм успешно активирован.");
                user!.State = State.Main;
                instagram!.IsSelected = false;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                break;
            case LoginResult.ChallengeRequired:
                await client.SendTextMessageAsync(message.From!.Id,
                    $"Ошибка: ({x.ErrorMessage}). Проверьте код и введите его ещё раз.",
                    replyMarkup: MainKeyboard.Main);
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
                user!.State = State.Main;
                instagram.IsSelected = false;
                await serviceContainer.InstagramService.UpdateAsync(instagram);
                break;
        }

        await serviceContainer.UserService.UpdateAsync(user!);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return message.Type == MessageType.Text && user!.State == State.ChallengeRequiredAccept;
    }
}