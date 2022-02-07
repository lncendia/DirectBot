namespace DirectBot.Core.Extensions;

public static class StringExtension
{
    public static string ToHtmlStyle(this string text) =>
        text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}