using DirectBot.Core.Configuration;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;

public static class MainKeyboard
{
    private static readonly string[] Emodji =
        {"🏞", "🏔", "🏖", "🌋", "🏜", "🏕", "🌎", "🗽", "🌃", "☘", "🐲", "🌸", "🌓", "🍃", "☀", "☁"};

    private static readonly Random Random = new();

    public static readonly ReplyKeyboardMarkup MainReplyKeyboard = new(new List<List<KeyboardButton>>
    {
        new() {new KeyboardButton("🌇 Мои аккаунты"), new KeyboardButton("❤ Задачи")},
        new() {new KeyboardButton("💰 Подписки"), new KeyboardButton("📲 Наши проекты")},
        new() {new KeyboardButton("📄 Инструкция"), new KeyboardButton("🤝 Поддержка")}
    })
    {
        ResizeKeyboard = true,
        InputFieldPlaceholder = "Нажмите на нужную кнопку"
    };

    public static InlineKeyboardMarkup Projects(List<Project> projects) =>
        new(projects.Select(variableProject =>
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithUrl($"{Emodji[Random.Next(0, Emodji.Length)]} {variableProject.Name}",
                    variableProject.Link)
            }).ToList());

    public static readonly InlineKeyboardMarkup Main =
        new(InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu"));
}