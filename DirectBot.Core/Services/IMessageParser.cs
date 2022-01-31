namespace DirectBot.Core.Services;

public interface IMessageParser
{
    string Generate(List<string> messages, List<List<string>> vocabularies);
    List<string> Split(string message);
    List<List<string>> GetVocabularies(string message);
}