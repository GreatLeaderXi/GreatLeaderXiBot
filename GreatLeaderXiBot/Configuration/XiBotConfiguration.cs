namespace GreatLeaderXiBot.Configuration;

public class XiBotConfiguration
{
    public ExchangeConfiguration ExchangeConfiguration { get; init; } = null!;

    public TelegramConfiguration TelegramConfiguration { get; init; } = null!;
}
