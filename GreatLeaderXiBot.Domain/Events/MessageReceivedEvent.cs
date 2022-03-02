namespace GreatLeaderXiBot.Domain.Events
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Common.Enums;
    using Commands;

    public class MessageReceivedEvent : INotification
    {
        public object Payload { get; init; }

        public BotMessageSources Source { get; init; }

        public MessageReceivedEvent(object payload, BotMessageSources source)
        {
            Payload = payload;
            Source = source;
        }
    }

    public class MessageReceivedEventHandler : INotificationHandler<MessageReceivedEvent>
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public MessageReceivedEventHandler(IMediator mediator)
        {
            _mediator = mediator!;
        }

        #endregion

        #region Methods

        public async Task Handle(MessageReceivedEvent notification, CancellationToken cancellationToken)
        {
            var command = notification.Source switch
            {
                BotMessageSources.Telegram => _mediator.Send(new TelegramMessageCommand(notification.Payload)),
                _ => Task.CompletedTask
            };

            await command;
        }

        #endregion
    }
}
