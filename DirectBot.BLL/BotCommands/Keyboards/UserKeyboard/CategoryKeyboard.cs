using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;

public static class CategoryKeyboard
{
    public static InlineKeyboardMarkup Create(List<string> usernames)
    {
        var list = new List<List<InlineKeyboardButton>>();
        for (int i = 0; i < Math.Min(usernames.Count, 10); i++)
        {
            list.Add(new List<InlineKeyboardButton>
                {InlineKeyboardButton.WithUrl($"{i+1}. Подписаться ✅", usernames[i])});
        }

        list.Add(new List<InlineKeyboardButton> {InlineKeyboardButton.WithCallbackData("🔎 Проверить", "check")});
        return new InlineKeyboardMarkup(list);
    }

    public static InlineKeyboardMarkup FinalLink(string link)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Перейти ✅", link));
    }
}