using MediatR;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using GreatLeaderXiBot.Common.Constants;
using GreatLeaderXiBot.Common.Configuration;
using GreatLeaderXiBot.Domain.Telegram.Commands;

namespace GreatLeaderXiBot.Domain.Telegram.Events;

/// <summary>
/// Event for incoming Telegram messages
/// </summary>
/// <param name="Payload"></param>
public record TelegramMessageEvent(Update Payload, ExchangeConfiguration ExchangeConfiguration) : INotification;

/// <summary>
/// Handler for incoming Telegram messages
/// </summary>
public record TelegramMessageEventHandler(IMediator _mediator, ILogger<TelegramMessageEventHandler> _logger) : INotificationHandler<TelegramMessageEvent>
{
    public async Task Handle(TelegramMessageEvent @event, CancellationToken cancellationToken)
    {
        var handler = @event.Payload.Type switch
        {
            UpdateType.Message => OnMessageReceivedAsync(@event.Payload.Message!),
            UpdateType.CallbackQuery => OnCallbackQueryReceivedAsync(@event.Payload.CallbackQuery!, @event.ExchangeConfiguration!),

            _ => Unit.Task
        };

        await handler;
    }

    private async Task OnMessageReceivedAsync(Message message)
    {
        _logger.LogInformation("Receive message from Telegram type: {messageType} {messageText}", message.Type, message.Text);

        // ignore all non-text messages for now
        if (message.Type != MessageType.Text)
            return;

        if (message.Text == "/start")
        {
            await _mediator.Send(new TelegramStartCommand(message));
        }
    }

    private async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery, ExchangeConfiguration exchangeConfiguration)
    {
        _logger.LogInformation($"Receive callback query from Telegram with payload: {callbackQuery.Data}");

        // ignore empty callback data
        if (String.IsNullOrEmpty(callbackQuery.Data))
            return;

        if (callbackQuery.Data == TelegramCallbackIds.GET_OUTLOOK_APPOINTMENTS)
        {
            await _mediator.Send(new TelegramAppointmentsCommand(callbackQuery, exchangeConfiguration));
        }
    }
}
