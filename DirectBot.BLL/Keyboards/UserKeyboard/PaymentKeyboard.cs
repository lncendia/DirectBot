using DirectBot.Core.DTO;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public class PaymentKeyboard
{
    public static readonly InlineKeyboardMarkup Subscribes =
        new(InlineKeyboardButton.WithCallbackData("⏱ Мои подписки", "subscribes"));

    public static InlineKeyboardMarkup CheckBill(Payment payment)
    {
        return new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithUrl("Оплатить", payment.PayUrl!)},
            new() {InlineKeyboardButton.WithCallbackData($"Проверить оплату", $"bill_{payment.Id}")},
        });
    }
}