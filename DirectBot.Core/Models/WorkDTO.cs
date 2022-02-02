using DirectBot.Core.Enums;

namespace DirectBot.Core.Models;

public class WorkDto
{
    public int Id { get; set; }
    public InstagramDto? Instagram { get; set; }
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
        string typeString = Type switch
        {
            WorkType.Subscriptions => "Тип: <code>Подписки</code>",
            WorkType.Subscribers => "Тип: <code>Подписxbrb</code>",
            WorkType.Hashtag => $"Тип: <code>Хештег ({Hashtag})</code>",
            WorkType.File => "Тип: <code>Файл</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        string workString =
            $"Работа №<code>{Id}</code>\n{typeString}\nКоличество пользователей: <code>{CountUsers}</code>\nВремя начала: <code>{StartTime.ToString("g")}</code>\nИнстаграм: <code>{Instagram?.Username}</code>\nСообщение: <code>{Message}</code>\nИнтервал: <code>{LowerInterval}:{UpperInterval}</code>\nЗавершена: <code>{(IsCompleted ? "Да" : "Нет")}</code>\n";
        if (CountSuccess != 0) workString += $"\nКоличество отправленных: <code>{CountSuccess}</code>";
        if (CountErrors != 0)
            workString += $"\nНе удалось отправить: <code>{CountErrors}</code>";
        if (!string.IsNullOrEmpty(ErrorMessage) && (IsSucceeded || !IsCompleted))
            workString += $"\nПоследняя ошибка: <code>{ErrorMessage}</code>";
        switch (IsCompleted)
        {
            case true when IsSucceeded:
                workString += "\nУспешно: <code>Да</code>";
                break;
            case true:
                workString += $"\nУспешно: <code>Нет</code>\nОшибка: <code>{ErrorMessage}</code>";
                break;
        }
        if(IsCanceled) workString += $"\n<b>Отменена</b>";

        return workString;
    }
}