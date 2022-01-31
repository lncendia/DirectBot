using DirectBot.Core.DTO;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public static class PaymentKeyboard
{
    public static readonly InlineKeyboardMarkup Subscribes = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("➕ Оплатить подписку", "buySubscribe")},
            new() {InlineKeyboardButton.WithCallbackData("⏱ Мои подписки", "mySubscribes_1")},
            new() {InlineKeyboardButton.WithCallbackData("💵 Мои платежи", "paymentsHistory_1")}
        });

    public static readonly InlineKeyboardMarkup PaySubscribe =
        new(InlineKeyboardButton.WithCallbackData("➕ Оплатить подписку", "buySubscribe"));

    public static InlineKeyboardMarkup ActivePayments(int page) =>
        new(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"paymentsHistory_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("➡", $"paymentsHistory_{page + 1}")
        });

    public static InlineKeyboardMarkup ActiveSubscribes(int page) =>
        new(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"mySubscribes_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("➡", $"mySubscribes_{page + 1}")
        });

    public static InlineKeyboardMarkup CheckBill(Payment payment) =>
        new(new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithUrl("Оплатить", payment.PayUrl!)},
            new() {InlineKeyboardButton.WithCallbackData("Проверить оплату", $"bill_{payment.Id}")},
        });
}