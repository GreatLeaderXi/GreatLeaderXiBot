namespace GreatLeaderXiBot.Common.Extensions;

using System.Text;

public static class StringBuilderExtensions
{
    public static string ToStringAndEscape(this StringBuilder sb)
    {
        return sb
            .Replace(".", "\\.")
            .Replace("-", "\\-")
            .Replace("+", "\\+")
            .Replace("!", "\\!")
            .Replace("<", "\\<")
            .Replace(">", "\\>")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .ToString();
    }
}
