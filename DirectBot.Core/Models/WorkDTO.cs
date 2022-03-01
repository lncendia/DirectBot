using DirectBot.Core.Enums;
using DirectBot.Core.Extensions;

namespace DirectBot.Core.Models;

public class WorkDto
{
    public int Id { get; set; }
    public List<InstagramLiteDto> Instagrams { get; set; } = new();
    public List<long> InstagramPks { get; set; } = new();
    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? JobId { get; set; }
    public string? Hashtag { get; set; }
    public string? FileIdentifier { get; set; }
    public int CountUsers { get; set; }
    public WorkUsersType UsersType { get; set; }
    public WorkType Type { get; set; }
    public DateTime StartTime { get; set; }

    public int CountPerDivision { get; set; }
    public TimeSpan IntervalPerDivision { get; set; }

    public bool IsCompleted { get; set; }

    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }


    public override string ToString()
    {
        var usersTypeString = UsersType switch
        {
            WorkUsersType.Subscriptions => "Получатели: <code>Подписки</code>",
            WorkUsersType.Subscribers => "Получатели: <code>Подписчики</code>",
            WorkUsersType.Hashtag => $"Получатели: <code>Хештег ({Hashtag?.ToHtmlStyle()})</code>",
            WorkUsersType.File => "Получатели: <code>Файл</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        var typeString = Type switch
        {
            WorkType.Simple => "Задача: <code>Рассылка</code>",
            WorkType.Divide => "Задача: <code>Разделение</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        string workString =
            $"Работа №<code>{Id}</code>\n{typeString}\n\n{usersTypeString}\nКоличество пользователей: <code>{CountUsers}</code>\nВремя начала: <code>{StartTime.ToString("g")}</code>\nИнстаграмы: <code>{string.Join(", ", Instagrams.Select(dto => dto.Username)).ToHtmlStyle()}</code>\nСообщение: <code>{Message?.ToHtmlStyle()}</code>\nИнтервал: <code>{LowerInterval}:{UpperInterval}</code>\nЗавершена: <code>{(IsCompleted ? "Да" : "Нет")}</code>\n";
        workString +=
            $"\nПользователей всего: <code>{(InstagramPks.Any() ? InstagramPks.Count : "Получение...")}</code>";
        switch (Type)
        {
            case WorkType.Simple:
            {
                if (CountSuccess != 0) workString += $"\nКоличество отправленных: <code>{CountSuccess}</code>";
                if (CountErrors != 0) workString += $"\nНе удалось отправить: <code>{CountErrors}</code>";
                break;
            }
            case WorkType.Divide:
            {
                if (CountSuccess != 0) workString += $"\nКоличество запущеных работ: <code>{CountSuccess}</code>";
                if (CountErrors != 0) workString += $"\nНе удалось запустить: <code>{CountErrors}</code>";
                break;
            }
        }

        if (!string.IsNullOrEmpty(ErrorMessage))
            workString += $"\nПоследняя ошибка: <code>{ErrorMessage.ToHtmlStyle()}</code>";
        if (IsCompleted)
            workString += $"\nУспешно: <code>{(CountErrors < CountSuccess ? "Да" : "Нет")}</code>";

        return workString;
    }
}

public class WorkLiteDto
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? Hashtag { get; set; }
    public string? FileIdentifier { get; set; }
    public int CountUsers { get; set; }
    public WorkUsersType UsersType { get; set; }
    public WorkType Type { get; set; }
    public DateTime StartTime { get; set; }

    public int CountPerDivision { get; set; }
    public TimeSpan IntervalPerDivision { get; set; }

    public bool IsCanceled { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }
}