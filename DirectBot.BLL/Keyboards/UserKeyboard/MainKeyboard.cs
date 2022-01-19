using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public class MainKeyboard
{
    public static readonly ReplyKeyboardMarkup MainReplyKeyboard = new(new List<List<KeyboardButton>>
    {
        new() {new KeyboardButton("🌇 Мои аккаунты"), new KeyboardButton("❤ Задачи")},
        new() {new KeyboardButton("💰 Оплатить подписку"), new KeyboardButton("🗒 Мой профиль")},
        new() {new KeyboardButton("📄 Инструкция"), new KeyboardButton("🤝 Поддержка")}
    })
    {
        ResizeKeyboard = true,
        InputFieldPlaceholder = "Нажмите на нужную кнопку"
    };


    public static InlineKeyboardMarkup Back(string query)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🔙 Назад", $"back_{query}"));
    }

    public static readonly InlineKeyboardMarkup Main =
        new(InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu"));
}