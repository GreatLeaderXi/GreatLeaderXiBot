using System.Text;
using MediatR;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using GreatLeaderXiBot.Common.Outlook;
using GreatLeaderXiBot.Common.Constants;
using GreatLeaderXiBot.Common.Extensions;

namespace GreatLeaderXiBot.Domain.Telegram.Commands;

/// <summary>
/// Acquire appointments for current & next day appointments
/// </summary>
/// <param name="CallbackQuery"></param>
public record TelegramAppointmentsCommand(CallbackQuery CallbackQuery) : IRequest;

/// <summary>
/// Handler
/// </summary>
public record TelegramAppointmentsCommandHandler(
    ITelegramBotClient BotClient, 
    ILogger<TelegramAppointmentsCommandHandler> Logger,
    IOutlookConnector OutlookConnector) : IRequestHandler<TelegramAppointmentsCommand>
{
    public async Task<Unit> Handle(TelegramAppointmentsCommand command, CancellationToken cancellationToken)
    {
        var appointsmentsMsg = await GetFormattedAppointmentsReplyMessageAsync();

        await BotClient.SendChatActionAsync(command.CallbackQuery.Message!.Chat.Id, ChatAction.Typing, cancellationToken);
        await BotClient.SendTextMessageAsync(
            chatId: command.CallbackQuery.Message!.Chat.Id,
            text: appointsmentsMsg,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI КАМЕНЬ <<", TelegramCallbackIds.GET_OUTLOOK_APPOINTMENTS)),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private async Task<string> GetFormattedAppointmentsReplyMessageAsync()
    {
        var sb = new StringBuilder("*Социальность рейтинг поднятие:*");
        sb.AppendLine();
        sb.AppendLine();

        foreach (var appointment in await OutlookConnector.GetAppointmentsAsync(DateTime.Today, DateTime.Today.AddDays(1)))
        {
            sb.AppendLine($"{appointment.Start:dd/MM/yyyy HH:mm}-{appointment.End: HH:mm}   _{appointment.Subject}_");
        }

        sb.AppendLine();
        sb.AppendLine("+20 социальный рейтинг 红龙习近平.");

        return sb.ToStringAndEscape();
    }
}
