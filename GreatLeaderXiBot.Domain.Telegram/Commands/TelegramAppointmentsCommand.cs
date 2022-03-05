using System.Text;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using GreatLeaderXiBot.Common.Outlook;
using GreatLeaderXiBot.Common.Constants;
using GreatLeaderXiBot.Common.Extensions;
using GreatLeaderXiBot.Common.Configuration;
using GreatLeaderXiBot.Common.Outlook.Dtos;

namespace GreatLeaderXiBot.Domain.Telegram.Commands;

/// <summary>
/// Acquire appointments for current & next day appointments
/// </summary>
/// <param name="CallbackQuery"></param>
public record TelegramAppointmentsCommand(CallbackQuery CallbackQuery, ExchangeConfiguration ExchangeConfig) : IRequest;

/// <summary>
/// Handler
/// </summary>
public record TelegramAppointmentsCommandHandler(
    ITelegramBotClient BotClient,
    IConfiguration Configuration,
    ILogger<TelegramAppointmentsCommandHandler> Logger,
    IOutlookConnector OutlookConnector) : IRequestHandler<TelegramAppointmentsCommand>
{
    public async Task<Unit> Handle(TelegramAppointmentsCommand command, CancellationToken cancellationToken)
    {
        var outlookSettings = new OutlookSettings(
            command.ExchangeConfig.ExchangeHost, 
            command.ExchangeConfig.ExchangeUsername, 
            command.ExchangeConfig.ExchangePassword);

        // calculate DateTime.Now in target user's timezone - could be better ways to do this
        //
        var utcOffset = new TimeSpan(command.ExchangeConfig.UtcOffset, 0, 0);

        var userTimezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.BaseUtcOffset == utcOffset) ?? TimeZoneInfo.Local;
        var userTimezoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimezone);

        var appointmentsDateStart = userTimezoneTime.Hour > 18 ? userTimezoneTime : userTimezoneTime.Date;
        var appointmentsDateEnd = userTimezoneTime.Hour > 18 ? userTimezoneTime.Date : userTimezoneTime.AddHours(18 - userTimezoneTime.Hour);

        var appointments = await OutlookConnector.GetAppointmentsAsync(appointmentsDateStart, appointmentsDateEnd, outlookSettings);

        var appointsmentsMsg = GetFormattedAppointmentsReplyMessage(appointments, userTimezone!, userTimezoneTime);

        await BotClient.SendChatActionAsync(command.CallbackQuery.Message!.Chat.Id, ChatAction.Typing, cancellationToken);
        await BotClient.SendTextMessageAsync(
            chatId: command.CallbackQuery.Message!.Chat.Id,
            text: appointsmentsMsg,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI КАМЕНЬ <<", TelegramCallbackIds.GET_OUTLOOK_APPOINTMENTS)),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private string GetFormattedAppointmentsReplyMessage(IEnumerable<OutlookAppointmentDto> appointments, TimeZoneInfo timezone, DateTime timeNowInTimezone)
    {
        var sb = new StringBuilder($"*Социальность рейтинг поднятие {timeNowInTimezone:dd/MM/yyyy}:*");
        
        if (appointments.Any())
        {
            sb.AppendLine();
            sb.AppendLine();

            foreach (var appointment in appointments)
            {
                sb.AppendLine($"{TimeZoneInfo.ConvertTime(appointment.Start, timezone):HH:mm}-{TimeZoneInfo.ConvertTime(appointment.End, timezone):HH:mm}   _{appointment.Subject}_");
            }
        }

        sb.AppendLine();
        sb.AppendLine($"+{appointments.Count() * 10} социальный рейтинг 红龙习近平.");

        return sb.ToStringAndEscape();
    }
}
