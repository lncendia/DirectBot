using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class StartEnterAccountDataQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        if (user!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }
        
        var instagrams = await serviceContainer.InstagramService.GetUserInstagramsCountAsync(user.Id);
        var subscribes = await serviceContainer.SubscribeService.GetUserSubscribesCountAsync(user.Id);
        if (instagrams >= subscribes)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет доступных подписок.");
            return;
        }

        user.State = State.EnterInstagramData;
        await serviceContainer.UserService.UpdateAsync(user);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите логин и пароль в формате: <code>[логин:пароль]</code>", ParseMode.Html,
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data == "enterData";
    }
}