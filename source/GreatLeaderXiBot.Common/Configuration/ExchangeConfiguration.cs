namespace GreatLeaderXiBot.Common.Configuration;

public class ExchangeConfiguration
{
    public string Host { get; init; } = null!;

    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;

    public int UserUtcOffset { get; init; }

    public int AppointmentsUtcOffset { get; init; }
}
