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
    public WorkType Type { get; set; }
    public DateTime StartTime { get; set; }
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
            $"Работа №<code>{Id}</code>\n{typeString}\nВремя начала: <code>{StartTime.ToString("g")}</code>\nИнстаграм: <code>{Instagram?.Username}</code>\nСообщение: <code>{Message}</code>\nИнтервал: <code>{LowerInterval}:{UpperInterval}</code>\nЗавершена: <code>{(IsCompleted ? "Да" : "Нет")}</code>\n";
        if (!IsCompleted) return workString;
        if (IsSucceeded)
        {
            workString += $"Успешно: <code>Да</code>\nКоличество отправленных: <code>{CountSuccess}</code>";
            if (CountErrors == 0) return workString;
            workString +=
                $"\nНе удалось отправить: <code>{CountErrors}</code>\nНаиболее частая ошибка: <code>{ErrorMessage}</code>";
        }
        else
            workString += $"Успешно: <code>Нет</code>\nОшибка: <code>{ErrorMessage}</code>";

        return workString;
    }
}