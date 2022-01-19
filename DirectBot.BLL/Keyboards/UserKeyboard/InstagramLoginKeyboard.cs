using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public static class InstagramLoginKeyboard
{
    public static InlineKeyboardMarkup Email(string email)
    {
        return new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new()
                {InlineKeyboardButton.WithCallbackData($"✉ Эл. адресс ({email})", "challengeEmail")},
            new()
                {InlineKeyboardButton.WithCallbackData("✅ Я подтвердил вход в инстаграме", "acceptEntry")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")}
        });
    }

    public static InlineKeyboardMarkup Phone(string number)
    {
        return new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new()
                {InlineKeyboardButton.WithCallbackData($"📲 Телефон ({number})", "challengePhone")},
            new()
                {InlineKeyboardButton.WithCallbackData("✅ Я подтвердил вход в инстаграме", "acceptEntry")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")}
        });
    }

    public static InlineKeyboardMarkup PhoneAndEmail(string email, string number)
    {
        return new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new()
                {InlineKeyboardButton.WithCallbackData($"📲 Телефон ({number})", "challengePhone")},
            new()
                {InlineKeyboardButton.WithCallbackData($"✉ Эл. адресс ({email})", "challengeEmail")},
            new()
                {InlineKeyboardButton.WithCallbackData("✅ Я подтвердил вход в инстаграме", "acceptEntry")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")}
        });
    }

    public static InlineKeyboardMarkup Exit(long id)
    {
        var keyboard = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{id}"),
            InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{id}")
        };
        return new InlineKeyboardMarkup(keyboard);
    }

    public static InlineKeyboardMarkup InstagramMain(long id, bool isActive)
    {
        List<InlineKeyboardButton> keyboard;
        if (isActive)
            keyboard = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{id}"),
                InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{id}")
            };
        else
            keyboard = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{id}"),
                InlineKeyboardButton.WithCallbackData("🚪 Удалить", $"exit_{id}"),
            };
        return new InlineKeyboardMarkup(keyboard);
    }


    public static InlineKeyboardMarkup Activate(long id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{id}"));
    }
}