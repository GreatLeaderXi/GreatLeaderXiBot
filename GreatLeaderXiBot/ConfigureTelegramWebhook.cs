using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using GreatLeaderXiBot.Common.Configuration;

namespace GreatLeaderXiBot;

public class ConfigureTelegramWebhook : IHostedService
{
    private readonly ILogger<ConfigureTelegramWebhook> _logger;
    private readonly IServiceProvider _services;
    private readonly TelegramConfiguration _botConfig;

    public ConfigureTelegramWebhook(ILogger<ConfigureTelegramWebhook> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _services = serviceProvider;
        _botConfig = configuration.GetSection("XiBotConfiguration").Get<XiBotConfiguration>().TelegramConfiguration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAddress = @$"{_botConfig.Host}/bot/{_botConfig.Token}";

        _logger.LogInformation("Setting webhook: {webhookAddress}", webhookAddress);

        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Removing webhook");

        await botClient.DeleteWebhookAsync();
    }
}
