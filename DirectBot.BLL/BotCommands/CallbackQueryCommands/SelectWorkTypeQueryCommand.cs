using DirectBot.BLL.BotCommands.Interfaces;
using DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;
using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DirectBot.BLL.BotCommands.CallbackQueryCommands;

public class SelectWorkTypeQueryCommand : ICallbackQueryCommand
{
    public async Task Execute(ITelegramBotClient client, UserDto? user, CallbackQuery query,
        ServiceContainer serviceContainer)
    {
        var type = (WorkType) Enum.Parse(typeof(WorkType), query.Data![9..]);
        if (user!.CurrentWork == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Работа не выбрана. Попробуйте ещё раз.");
            return;
        }

        user.CurrentWork.Type = type;

        switch (type)
        {
            case WorkType.Simple:
                user.State = State.EnterCountUsers;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите число получателей. Должно быть не менее 1 и не более 500 для обычных и 1500 для разделенных работ.",
                    replyMarkup: MainKeyboard.Main);
                break;
            case WorkType.Divide:
                user.State = State.EnterDivideData;
                await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                    "Введите данные, необходимые для разделения работы в формате:\n<code>[число пользователей в подзадачах-интервал между подзадачами]</code>\nФормат даты: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>\nПример: <code>50-12:00</code>",
                    ParseMode.Html,
                    replyMarkup: MainKeyboard.Main);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await serviceContainer.UserService.UpdateAsync(user);
    }

    public bool Compare(CallbackQuery query, UserDto? user)
    {
        return query.Data!.StartsWith("workType") && user!.State == State.SelectTypeWork;
    }
}