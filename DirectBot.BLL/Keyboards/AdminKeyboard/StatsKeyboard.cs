using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.AdminKeyboard;

public static class StatsKeyboard
{
    public static readonly InlineKeyboardMarkup Stats = new(new List<List<InlineKeyboardButton>>
    {
        new() {InlineKeyboardButton.WithCallbackData("За сегодня", "stats_today")},
        new() {InlineKeyboardButton.WithCallbackData("За неделю", "stats_week")},
        new() {InlineKeyboardButton.WithCallbackData("За месяц", "stats_month")},
        new() {InlineKeyboardButton.WithCallbackData("За все время", "stats_ever")},
    });
}