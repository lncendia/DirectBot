using DirectBot.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.Keyboards.UserKeyboard;

public static class WorkingKeyboard
{
    private static readonly string[] Emodji =
        {"🏞", "🏔", "🏖", "🌋", "🏜", "🏕", "🌎", "🗽", "🌃", "☘", "🐲", "🌸", "🌓", "🍃", "☀", "☁"};

    public static readonly InlineKeyboardMarkup Working = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("🏃 Начать задачу", "startWorking")},
            new() {InlineKeyboardButton.WithCallbackData("⚙ Активные задачи", "worksHistory_1")}
        });

    public static readonly InlineKeyboardMarkup StartWork = new(
        new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("🏃 Начать сейчас", "startNow"),
                InlineKeyboardButton.WithCallbackData("⌛ Начать позже", "startLater")
            },
            new() {InlineKeyboardButton.WithCallbackData("🛑 Отмена", "mainMenu")}
        });

    public static InlineKeyboardMarkup Select(IEnumerable<InstagramDto> instagrams)
    {
        var accounts = instagrams.Select(inst => new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData($"{Emodji[new Random().Next(0, Emodji.Length)]} {inst.Username}",
                $"select_{inst.Id}")
        }).ToList();

        accounts.Add(new List<InlineKeyboardButton>
            {InlineKeyboardButton.WithCallbackData("🗒 Выбрать все аккаунты", "selectAll")});
        accounts.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("👈 Подтвердить выбор", "continueSelect"),
            InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")
        });

        return new InlineKeyboardMarkup(accounts);
    }

    public static InlineKeyboardMarkup NewSelect(List<IEnumerable<InlineKeyboardButton>?> keyboard, string query)
    {
        keyboard.Remove(keyboard.FirstOrDefault(list => list!.Any(key => key.CallbackData == query)));
        if (keyboard.Count == 2)
            keyboard.Remove(keyboard.FirstOrDefault(list => list!.Any(key => key.CallbackData == "selectAll")));

        return new InlineKeyboardMarkup(keyboard!);
    }

    public static InlineKeyboardMarkup ActiveWorks(int page, WorkDto workDto)
    {
        var list = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"worksHistory_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("🔄", $"restartWork_{workDto.Id}"),
            InlineKeyboardButton.WithCallbackData("➡", $"worksHistory_{page + 1}")
        };

        if (!workDto.IsCompleted && !string.IsNullOrEmpty(workDto.JobId))
            list.Insert(2, InlineKeyboardButton.WithCallbackData("⏹", $"stopWork_{workDto.Id}"));
        return new InlineKeyboardMarkup(list);
    }
}