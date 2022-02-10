using DirectBot.Core.Enums;
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
            new() {InlineKeyboardButton.WithCallbackData("⚙ Мои задачи", "worksHistory_1")}
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

    public static InlineKeyboardMarkup Select(IEnumerable<InstagramLiteDto> instagrams)
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

        if (!workDto.IsCompleted)
            list.Insert(2, InlineKeyboardButton.WithCallbackData("⏹", $"stopWork_{workDto.Id}"));
        return new InlineKeyboardMarkup(list);
    }

    public static readonly InlineKeyboardMarkup SelectType = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("1️⃣ Подписчики", $"type_{WorkType.Subscribers.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("2️⃣ Подписки", $"type_{WorkType.Subscriptions.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("3️⃣ Хештег", $"type_{WorkType.Hashtag.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("4️⃣ Файл", $"type_{WorkType.File.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static readonly InlineKeyboardMarkup SelectTypeForManyAccount = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("1️⃣ Хештег", $"type_{WorkType.Hashtag.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("2️⃣ Файл", $"type_{WorkType.File.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static InlineKeyboardMarkup StopWork(int id) =>
        new(InlineKeyboardButton.WithCallbackData("⏹ Остановить", $"stopWork_{id}"));
}