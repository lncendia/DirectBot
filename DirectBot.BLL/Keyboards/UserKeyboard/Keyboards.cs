using DirectBot.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public static class Keyboards
{
    public static readonly string[] Emodji =
        {"🏞", "🏔", "🏖", "🌋", "🏜", "🏕", "🌎", "🗽", "🌃", "☘", "🐲", "🌸", "🌓", "🍃", "☀", "☁"};
    

    public static readonly InlineKeyboardMarkup EnterData = new(
        InlineKeyboardButton.WithCallbackData("🖊 Ввести данные", "enterData"));

    public static InlineKeyboardMarkup ChangeProxy(InstagramDTO instagram)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("♻ Сменить прокси", $"changeProxy_{instagram.Id}"));
    }

    public static InlineKeyboardMarkup Cancel(long id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🛑 Отменить", $"cancel_{id}"));
    }

}