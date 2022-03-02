namespace GreatLeaderXiBot
{
    using Telegram.Bot;
    using Telegram.Bot.Types.Enums;

    using Configuration;
    public class ConfigureTelegramWebhook : IHostedService
    {
        #region Fields

        private readonly ILogger<ConfigureTelegramWebhook> _logger;
        private readonly IServiceProvider _services;
        private readonly TelegramConfiguration _botConfig;

        #endregion

        #region Constructors

        public ConfigureTelegramWebhook(
            ILogger<ConfigureTelegramWebhook> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _services = serviceProvider;
            _botConfig = configuration.GetSection("XiBotConfiguration").Get<XiBotConfiguration>().TelegramConfiguration;
        }

        #endregion

        #region Methods

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = @$"{_botConfig.Host}/bot/{_botConfig.Token}/";

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

            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }

        #endregion
    }
}
