using DirectBot.BLL.Interfaces;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.TextCommands;

public class EnterMessageToMailingCommand : ITextCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, Message message, ServiceContainer serviceContainer)
    {
        var users = await serviceContainer.UserService.GetAllAsync();
        switch (message.Type)
        {
            case MessageType.Text:
                var tasks = users.Select(user1 => client.SendTextMessageAsync(user1.Id, message.Text!));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Photo:
                tasks = users.Select(user1 => client.SendPhotoAsync(user1.Id,
                    new InputMedia(message.Photo!.Last().FileId), message.Caption));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Audio:
                tasks = users.Select(user1 =>
                    client.SendAudioAsync(user1.Id, new InputMedia(message.Audio!.FileId)));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Video:
                tasks = users.Select(user1 => client.SendVideoAsync(user1.Id, new InputMedia(message.Video!.FileId),
                    caption: message.Caption));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Voice:
                tasks = users.Select(user1 => client.SendVoiceAsync(user1.Id, new InputMedia(message.Voice!.FileId)));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Document:
                tasks = users.Select(user1 => client.SendDocumentAsync(user1.Id,
                    new InputMedia(message.Document!.FileId)));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
            case MessageType.Sticker:
                tasks = users.Select(user1 => client.SendPhotoAsync(user1.Id, new InputMedia(message.Sticker!.FileId)));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // ignored
                }

                break;
        }

        GC.Collect();
        await client.SendTextMessageAsync(user!.Id,
            "Сообщение было успешно отправлено. Вы в главном меню.");
        user!.State = State.Main;
        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(Message message, UserDto? user)
    {
        return user!.State == State.MailingAdmin;
    }
}