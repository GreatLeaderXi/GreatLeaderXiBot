namespace GreatLeaderXiBot.Domain.Commands
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using MediatR;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.InputFiles;
    using Telegram.Bot.Types.ReplyMarkups;

    using Common.Extensions;

    using Core.Contracts;

    public class TelegramMessageCommand : IRequest
    {
        public object Payload { get; init; }

        public TelegramMessageCommand(object payload)
        {
            Payload = payload;
        }
    }

    public class TelegramMessageCommandHandler : IRequestHandler<TelegramMessageCommand>
    {
        #region Fields

        private readonly ITelegramBotClient _botClient;
        private readonly IOutlookConnector _outlookConnector;
        private readonly ILogger<TelegramMessageCommandHandler> _logger;

        #endregion

        #region Constructors

        public TelegramMessageCommandHandler(ITelegramBotClient botClient, IOutlookConnector outlookConnector, ILogger<TelegramMessageCommandHandler> logger)
        {
            _botClient = botClient;
            _outlookConnector = outlookConnector;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<Unit> Handle(TelegramMessageCommand request, CancellationToken cancellationToken)
        {
            if (request.Payload is Update update)
            {
                var handler = update.Type switch
                {
                    UpdateType.Message => OnMessageReceivedAsync(update.Message!),
                    UpdateType.CallbackQuery => OnCallbackQueryReceivedAsync(update.CallbackQuery!),

                    _ => Unit.Task
                };

                await handler;
            }

            return Unit.Value;
        }

        private async Task OnMessageReceivedAsync(Message message)
        {
            _logger.LogInformation("Receive message type: {messageType} {messageText}", message.Type, message.Text);

            if (message.Type != MessageType.Text)
                return;

            if (message.Text == "/start")
            {
                var sb = new StringBuilder();
                sb.AppendLine($"*ПАРТИЯ* гордится простой рабочий {message.Chat.Username} город Тверь!");
                sb.AppendLine("Хотеть чтобы *Великий Xi* вести 中国 к славный Социалистический будущее!");
                sb.AppendLine("Помнить, что ты являться член Партий Китай и обязан вести себя подобающе");

                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await _botClient.SendStickerAsync(
                    message.Chat.Id,
                    sticker: new InputOnlineFile("CAACAgUAAxkBAAEEB-BiH1tUpXXoFrFodTjFq6BC2lj0RAAC0wUAAlQPhQEl3BszliDqpyME"));

                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await _botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    sb.Escape(),
                    parseMode: ParseMode.MarkdownV2);

                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await _botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Жать *КНОПКА* вниз для двойной рис порция удар:",
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI КАМЕНЬ <<", "HowMuchIsAFish")));
            }
        }

        private async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
        {
            if (String.IsNullOrEmpty(callbackQuery.Data))
                return;

            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message!.MessageId;

            if (callbackQuery.Data == "HowMuchIsAFish")
            {
                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                var appointments = _outlookConnector.GetAppointments(DateTime.Today, DateTime.Today.AddDays(1));

                var sb = new StringBuilder("*Социальность рейтинг поднятие:*");
                sb.AppendLine();
                sb.AppendLine();

                foreach (var appointment in appointments)
                {
                    sb.AppendLine($"{appointment.Start:dd/MM/yyyy HH:mm}-{appointment.End: HH:mm}   _{appointment.Subject}_");
                }

                sb.AppendLine();
                sb.AppendLine("+20 социальный рейтинг 红龙习近平.");

                await _botClient.SendChatActionAsync(chatId, ChatAction.Typing);
                await _botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: sb.Escape(),
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI КАМЕНЬ <<", "HowMuchIsAFish")));
            }
        }

        #endregion
    }
}
