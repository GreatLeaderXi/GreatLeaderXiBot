namespace GreatLeaderXiBot.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Telegram.Bot.Exceptions;

    using Common.Enums;
    using Domain.Events;
    using Telegram.Bot.Types;

    public class TelegramWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<TelegramWebhookController> logger,
            [FromBody] Update update)
        {
            try
            {
                await mediator.Publish(new MessageReceivedEvent(update, BotMessageSources.Telegram));
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
}
