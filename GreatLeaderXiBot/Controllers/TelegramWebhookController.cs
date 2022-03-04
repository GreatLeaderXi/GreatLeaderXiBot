namespace GreatLeaderXiBot.Controllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

using Domain.Telegram.Events;

public class TelegramWebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] IMediator mediator, [FromServices] ILogger<TelegramWebhookController> logger, [FromBody] Update update)
    {
        try
        {
            await mediator.Publish(new TelegramMessageEvent(update));
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, logger);
        }

        return Ok();
    }

    private Task HandleErrorAsync(Exception exception, ILogger<TelegramWebhookController> logger)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
}
