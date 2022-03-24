namespace GreatLeaderXiBot.Common.Extensions;

using System.Text;

public static class StringExtensions
{
    public static StringBuilder Escape(this StringBuilder sb)
    {
        return sb
            .Replace(".", "\\.")
            .Replace("-", "\\-")
            .Replace("+", "\\+")
            .Replace("!", "\\!")
            .Replace("<", "\\<")
            .Replace(">", "\\>")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    public static string Escape(this string s)
    {
        return s
            .Replace(".", "\\.")
            .Replace("-", "\\-")
            .Replace("+", "\\+")
            .Replace("!", "\\!")
            .Replace("<", "\\<")
            .Replace(">", "\\>")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }
}
