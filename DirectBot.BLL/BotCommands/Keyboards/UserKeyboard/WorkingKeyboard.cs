using DirectBot.Core.Enums;
using DirectBot.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectBot.BLL.BotCommands.Keyboards.UserKeyboard;

public static class WorkingKeyboard
{
    private static readonly string[] Emodji =
        {"🏞", "🏔", "🏖", "🌋", "🏜", "🏕", "🌎", "🗽", "🌃", "☘", "🐲", "🌸", "🌓", "🍃", "☀", "☁"};

    private static readonly Random Random = new();

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
            InlineKeyboardButton.WithCallbackData($"{Emodji[Random.Next(0, Emodji.Length)]} {inst.Username}",
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
            InlineKeyboardButton.WithCallbackData("🔄", $"selectRestart_{workDto.Id}"),
            InlineKeyboardButton.WithCallbackData("➡", $"worksHistory_{page + 1}")
        };

        if (!workDto.IsCompleted)
            list.Insert(2, InlineKeyboardButton.WithCallbackData("⏹", $"stopWork_{workDto.Id}"));
        return new InlineKeyboardMarkup(list);
    }

    public static readonly InlineKeyboardMarkup SelectUserType = new(
        new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("1️⃣ Подписчики",
                    $"usersType_{WorkUsersType.Subscribers.ToString()}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("2️⃣ Подписки",
                    $"usersType_{WorkUsersType.Subscriptions.ToString()}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("3️⃣ Хештег", $"usersType_{WorkUsersType.Hashtag.ToString()}")
            },
            new() {InlineKeyboardButton.WithCallbackData("4️⃣ Файл", $"usersType_{WorkUsersType.File.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static readonly InlineKeyboardMarkup SelectUserTypeForManyAccount = new(
        new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("1️⃣ Хештег", $"usersType_{WorkUsersType.Hashtag.ToString()}")
            },
            new() {InlineKeyboardButton.WithCallbackData("2️⃣ Файл", $"usersType_{WorkUsersType.File.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static readonly InlineKeyboardMarkup SelectTypeWork = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("1️⃣ Обычная", $"workType_{WorkType.Simple.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("2️⃣ Разделить", $"workType_{WorkType.Divide.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static InlineKeyboardMarkup StopWork(int id) =>
        new(InlineKeyboardButton.WithCallbackData("⏹ Остановить", $"stopWork_{id}"));

    public static InlineKeyboardMarkup Restart(int id)
    {
        return new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("👨‍🔧 Тем же", $"restartWork_1_{id}")},
            new() {InlineKeyboardButton.WithCallbackData("🙅‍♂ Заново", $"restartWork_0_{id}")}
        });
    }
}