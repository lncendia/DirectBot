using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;

public static class MainKeyboard
{
    public static readonly ReplyKeyboardMarkup MainReplyKeyboard = new(new List<List<KeyboardButton>>
    {
        new() {new KeyboardButton("🌇 Мои аккаунты"), new KeyboardButton("❤ Задачи")},
        new() {new KeyboardButton("💰 Подписки")},
        new() {new KeyboardButton("📄 Инструкция"), new KeyboardButton("🤝 Поддержка")}
    })
    {
        ResizeKeyboard = true,
        InputFieldPlaceholder = "Нажмите на нужную кнопку"
    };


    public static InlineKeyboardMarkup Back(string query) =>
        new(InlineKeyboardButton.WithCallbackData("🔙 Назад", $"back_{query}"));

    public static readonly InlineKeyboardMarkup Main =
        new(InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu"));
}