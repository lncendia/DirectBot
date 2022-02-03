using DirectBot.Core.Enums;

namespace DirectBot.Core.Models;

public class WorkDto
{
    public int Id { get; set; }
    public UserDto? User { get; set; }
    public List<InstagramDto> Instagrams { get; set; } = new();
    public string? Message { get; set; }
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public string? JobId { get; set; }
    public string? Hashtag { get; set; }
    public string? FileIdentifier { get; set; }
    public int CountUsers { get; set; }
    public WorkType Type { get; set; }
    public DateTime StartTime { get; set; }

    public bool IsCanceled { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }


    public override string ToString()
    {
        var typeString = Type switch
        {
            WorkType.Subscriptions => "Тип: <code>Подписки</code>",
            WorkType.Subscribers => "Тип: <code>Подписчики</code>",
            WorkType.Hashtag => $"Тип: <code>Хештег ({Hashtag})</code>",
            WorkType.File => "Тип: <code>Файл</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        string workString =
            $"Работа №<code>{Id}</code>\n{typeString}\nКоличество пользователей: <code>{CountUsers}</code>\nВремя начала: <code>{StartTime.ToString("g")}</code>\nИнстаграмы: <code>{string.Join(", ", Instagrams.Select(dto => dto.Username))}</code>\nСообщение: <code>{Message}</code>\nИнтервал: <code>{LowerInterval}:{UpperInterval}</code>\nЗавершена: <code>{(IsCompleted ? "Да" : "Нет")}</code>\n";
        if (CountSuccess != 0) workString += $"\nКоличество отправленных: <code>{CountSuccess}</code>";
        if (CountErrors != 0)
            workString += $"\nНе удалось отправить: <code>{CountErrors}</code>";
        if (!string.IsNullOrEmpty(ErrorMessage))
            workString += $"\nПоследняя ошибка: <code>{ErrorMessage}</code>";
        if (IsCompleted)
            workString += $"\nУспешно: <code>{(IsSucceeded ? "Да" : "Нет")}</code>";
        if (IsCanceled) workString += $"\n<b>Отменена</b>";

        return workString;
    }
}