using MediatR;
using Telegram.Bot;

using GreatLeaderXiBot;
using GreatLeaderXiBot.Configuration;
using GreatLeaderXiBot.Common.Outlook;
using GreatLeaderXiBot.Domain.Telegram.Events;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("XiBotConfiguration").Get<XiBotConfiguration>();

var exchangeConfig = botConfig.ExchangeConfiguration;
var telegramConfig = botConfig.TelegramConfiguration;

builder.Services.AddHostedService<ConfigureTelegramWebhook>();
builder.Services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(telegramConfig.Token, httpClient));

builder.Services.AddScoped<IOutlookConnector, OutlookConnector>(x => 
    new OutlookConnector(exchangeConfig.ExchangeHost, exchangeConfig.ExchangeUsername, exchangeConfig.ExchangePassword));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.OutputFormatterMemoryBufferThreshold = 48 * 1024;
});

builder.Services.AddMediatR(typeof(TelegramMessageEvent).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "tgwebhook", pattern: $"bot/{telegramConfig.Token}", new { controller = "TelegramWebhook", action = "Post" });
    endpoints.MapControllers();
});

app.Run();
