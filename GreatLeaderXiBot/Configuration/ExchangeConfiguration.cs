namespace GreatLeaderXiBot.Configuration;

public class ExchangeConfiguration
{
    public string ExchangeHost { get; init; } = null!;

    public string ExchangeUsername { get; init; } = null!;

    public string ExchangePassword { get; init; } = null!;

    public int UtcOffset { get; init; }
}
