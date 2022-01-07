using DirectBot.BLL.Interfaces;
using DirectBot.BLL.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DirectBot.Core.Models.User;

namespace DirectBot.BLL.CallbackQueryCommands;

public class CheckQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, User? user, CallbackQuery query, IUserService userService,
        
        Core.Configuration.Configuration configuration)
    {
        var tasks = channelService.GetAll().Select(link => client.GetChatMemberAsync(new ChatId(link.Id), user!.Id));
        try
        {
            var results = await Task.WhenAll(tasks);
            if (results.Any(result => result.Status == ChatMemberStatus.Left))
            {
                await client.AnswerCallbackQueryAsync(query.Id, "Вы не подисались на все каналы.", true);
                return;
            }

            await client.EditMessageTextAsync(user!.Id, query.Message!.MessageId,
                "✅ ДОСТУП ОТКРЫТ\n\nВсе фильмы загрузили на наш основной канал 👇",
                replyMarkup: CategoryKeyboard.FinalLink(configuration.FinalChanel));
        }
        catch (Exception ex)
        {
            await client.EditMessageTextAsync(user!.Id, query.Message!.MessageId,
                $"Не удалось произвести проверку: <code>{ex.Message}</code>.", ParseMode.Html);
        }
    }

    public bool Compare(CallbackQuery query, User? user)
    {
        return user!.State == State.Main && query.Data!.StartsWith("check");
    }
}