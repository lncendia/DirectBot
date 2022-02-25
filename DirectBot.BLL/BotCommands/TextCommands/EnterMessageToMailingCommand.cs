using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.TextCommands;

public class EnterMessageToMailingCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message,
        ServiceContainer serviceContainer)
    {
        var users = await serviceContainer.UserService.GetAllAsync();
        IEnumerable<Task<Message>> tasks = new List<Task<Message>>();
        switch (message.Type)
        {
            case MessageType.Text:
                tasks = users.Select(user1 =>
                    client.SendTextMessageAsync(user1.Id, message.Text!, ParseMode.Html));
                break;
            case MessageType.Photo:
                tasks = users.Select(user1 => client.SendPhotoAsync(user1.Id,
                    new InputMedia(message.Photo!.Last().FileId), message.Caption, ParseMode.Html));
                break;
            case MessageType.Audio:
                tasks = users.Select(user1 =>
                    client.SendAudioAsync(user1.Id, new InputMedia(message.Audio!.FileId), parseMode: ParseMode.Html));
                break;
            case MessageType.Video:
                tasks = users.Select(user1 => client.SendVideoAsync(user1.Id, new InputMedia(message.Video!.FileId),
                    caption: message.Caption, parseMode: ParseMode.Html));
                break;
            case MessageType.Voice:
                tasks = users.Select(user1 =>
                    client.SendVoiceAsync(user1.Id, new InputMedia(message.Voice!.FileId), parseMode: ParseMode.Html));
                break;
            case MessageType.Document:
                tasks = users.Select(user1 => client.SendDocumentAsync(user1.Id,
                    new InputMedia(message.Document!.FileId), caption: message.Caption, parseMode: ParseMode.Html));
                break;
            case MessageType.Sticker:
                tasks = users.Select(user1 =>
                    client.SendStickerAsync(user1.Id, new InputMedia(message.Sticker!.FileId)));
                break;
        }

        var task = Task.WhenAll(tasks);
        try
        {
            await task;
            await client.SendTextMessageAsync(user!.Id,
                $"Сообщение было успешно отправлено {users.Count} пользователю(ям). Вы в главном меню.");
        }
        catch (Exception)
        {
            int exceptionsCount = task.Exception?.InnerExceptions.Count ?? 0;
            await client.SendTextMessageAsync(user!.Id,
                $"Сообщение было отправлено {users.Count - exceptionsCount} пользователю(ям). У {exceptionsCount} пользователя(ей) возникла ошибка. Вы в главном меню.");
        }

        GC.Collect();
        user.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return user!.State == State.MailingAdmin;
    }
}