using System.Text.RegularExpressions;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class MessageParser : IMessageParser
{
    public string Generate(List<string> text, List<List<string>> vocabularies)
    {
        var rnd = new Random();
        var finalString = string.Empty;
        for (int i = 0; i < vocabularies.Count; i++)
        {
            finalString += text[i] + vocabularies[i][rnd.Next(0, vocabularies[i].Count)];
        }

        return finalString + text.Last();
    }

    public List<string> Split(string message) => Regex.Split(message, "{[a-zA-Zа-яА-Я_; ]+}").ToList();

    public List<List<string>> GetVocabularies(string message) =>
        Regex.Matches(message, "{[a-zA-Zа-яА-Я_; ]+}").Select(match => match.Value[1..^1].Split(';').ToList())
            .ToList();
}