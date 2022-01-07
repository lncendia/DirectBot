using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.AdminKeyboard;

public static class DeleteKeyboard
{
    public static InlineKeyboardMarkup Delete(List<(string? Username, long Id)> ids)
    {
        var rows = new List<List<InlineKeyboardButton>>();
        ids.ForEach(key =>
        {
            rows.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(key.Item1 ?? $"Имя отсутствует ({key.Id})", $"delete_{key.Item2.ToString()}")
            });
        });
        return new InlineKeyboardMarkup(rows);
    }
}