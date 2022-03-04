using System.Text;

using MediatR;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

using GreatLeaderXiBot.Common.Constants;
using GreatLeaderXiBot.Common.Extensions;

namespace GreatLeaderXiBot.Domain.Telegram.Commands;

/// <summary>
/// Greetings message
/// </summary
public record TelegramStartCommand(Message Message) : IRequest;

/// <summary>
/// Greetings message handler
/// </summary>
public record TelegramStartCommandHandler(ITelegramBotClient BotClient, ILogger<TelegramStartCommandHandler> Logger) : IRequestHandler<TelegramStartCommand>
{
    public async Task<Unit> Handle(TelegramStartCommand command, CancellationToken cancellationToken)
    {
        var chatId = command.Message.Chat.Id;

        await BotClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken);
        await BotClient.SendStickerAsync(
            chatId,
            sticker: new InputOnlineFile(TelegramStickersIds.GREAT_LEADER_XI),
            cancellationToken: cancellationToken);

        await BotClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken);
        await BotClient.SendTextMessageAsync(
            chatId,
            text: GetGreetingsMessage(command.Message),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        await BotClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken);
        await BotClient.SendTextMessageAsync(
            chatId,
            $"Жать *КНОПКА* вниз для двойной рис порция удар:",
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI КАМЕНЬ <<", TelegramCallbackIds.GET_OUTLOOK_APPOINTMENTS)),
            cancellationToken: cancellationToken); ;

        return Unit.Value;
    }

    private string GetGreetingsMessage(Message message)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"*ПАРТИЯ* гордится простой рабочий {message.Chat.Username} город Тверь!");
        sb.AppendLine("Хотеть чтобы *Великий Xi* вести 中国 к славный Социалистический будущее!");
        sb.AppendLine("Помнить, что ты являться член Партий Китай и обязан вести себя подобающе");

        return sb.ToStringAndEscape();
    }
}
