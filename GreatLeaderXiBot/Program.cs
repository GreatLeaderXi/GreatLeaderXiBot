using MediatR;
using Telegram.Bot;

using GreatLeaderXiBot;
using GreatLeaderXiBot.Configuration;

using GreatLeaderXiBot.Domain.Events;
using GreatLeaderXiBot.Domain.Outlook;
using GreatLeaderXiBot.Domain.Core.Contracts;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("XiBotConfiguration").Get<XiBotConfiguration>();

var exchangeConfig = botConfig.ExchangeConfiguration;
var telegramConfig = botConfig.TelegramConfiguration;

builder.Services.AddHostedService<ConfigureTelegramWebhook>();
builder.Services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(telegramConfig.Token, httpClient));

builder.Services.AddScoped<IOutlookConnector, OutlookConnector>(x => 
    new OutlookConnector(exchangeConfig.ExchangeHost, exchangeConfig.ExchangeUsername, exchangeConfig.ExchangePassword));

builder.Services.AddMediatR(typeof(MessageReceivedEvent).Assembly);
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.OutputFormatterMemoryBufferThreshold = 48 * 1024;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "tgwebhook", pattern: $"bot/{telegramConfig.Token}", new { controller = "TelegramWebhookController", action = "Post" });
    endpoints.MapControllers();
});

app.Run();
