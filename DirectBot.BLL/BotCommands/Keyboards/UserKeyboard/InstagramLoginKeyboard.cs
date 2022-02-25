using DirectBot.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;

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
        var keyboard = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{id}"),
            InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{id}")
        };
        return new InlineKeyboardMarkup(keyboard);
    }

    public static InlineKeyboardMarkup InstagramMain(InstagramLiteDto instagram)
    {
        var list = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("🖊", $"edit_{instagram.Id}"),
        };
        List<InlineKeyboardButton> keyboard;
        if (instagram.IsActive)
            keyboard = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{instagram.Id}"),
                InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{instagram.Id}")
            };
        else
            keyboard = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{instagram.Id}"),
                InlineKeyboardButton.WithCallbackData("🚪 Удалить", $"exit_{instagram.Id}"),
            };
        return new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>> {list, keyboard});
    }

    public static readonly InlineKeyboardMarkup MyAccounts = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("🆕 Добавить", "enterData")},
            new() {InlineKeyboardButton.WithCallbackData("🗒 Мои аккаунты", "myInstagrams")}
        });


    public static InlineKeyboardMarkup Activate(int id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{id}"));
    }

    public static InlineKeyboardMarkup Edit(int id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🖊 Редактировать", $"edit_{id}"));
    }
}