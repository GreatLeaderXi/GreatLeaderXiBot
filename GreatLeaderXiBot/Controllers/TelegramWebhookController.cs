namespace GreatLeaderXiBot.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Telegram.Bot.Exceptions;

    using Common.Enums;
    using Domain.Events;

    public class TelegramWebhookController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;
        private readonly ILogger<TelegramWebhookController> _logger;

        #endregion

        #region Constructors

        public TelegramWebhookController(IMediator mediator, ILogger<TelegramWebhookController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] object updateData)
        {
            try
            {
                await _mediator.Publish(new MessageReceivedEvent(updateData, BotMessageSources.Telegram));
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }

            return Ok();
        }

        private Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        #endregion
    }
}
