using System.Text;

using MediatR;
using NodaTime;

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
public record TelegramAppointmentsCommandHandler(ITelegramBotClient _bot, IConfiguration _config, IOutlookConnector _outlook) : IRequestHandler<TelegramAppointmentsCommand>
{
    private const int WORKING_DAY_START_HOUR = 8;
    private const int WORKING_DAY_END_HOUR = 18;

    public async Task<Unit> Handle(TelegramAppointmentsCommand command, CancellationToken cancellationToken)
    {
        var outlookSettings = new OutlookSettings(
            command.ExchangeConfig.ExchangeHost, 
            command.ExchangeConfig.ExchangeUsername, 
            command.ExchangeConfig.ExchangePassword);

        var userUtcOffset = Offset.FromHours(command.ExchangeConfig.UserUtcOffset);
        var userTimezone = DateTimeZone.ForOffset(userUtcOffset);

        var appointmentsUtcOffset = Offset.FromHours(command.ExchangeConfig.AppointmentsUtcOffset);
        var appointmentsTimezone = DateTimeZone.ForOffset(appointmentsUtcOffset);

        var now = SystemClock.Instance.GetCurrentInstant();
        var nowForUser = now.InZone(userTimezone);
        var nowForAppointments = now.InZone(appointmentsTimezone);

        (var startDate, var endDate) = MapCurrentTimeToWorkingDayTime(nowForAppointments);

        var appointments = await _outlook.GetAppointmentsAsync(startDate, endDate, outlookSettings);

        await _bot.SendChatActionAsync(command.CallbackQuery.Message!.Chat.Id, ChatAction.Typing, cancellationToken);
        await _bot.SendTextMessageAsync(
            chatId: command.CallbackQuery.Message!.Chat.Id,
            text: GetFormattedAppointmentsReplyMessage(appointments, nowForUser),
            parseMode: ParseMode.MarkdownV2,
            disableWebPagePreview: true,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(">> ПОЛУЧИТЬ НЕФРИТОВЫЙ СТЕРЖЕНЬ XI <<", TelegramCallbackIds.GET_OUTLOOK_APPOINTMENTS)),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }

    private (ZonedDateTime Start, ZonedDateTime End) MapCurrentTimeToWorkingDayTime(ZonedDateTime time)
    {
        var timezone = time.Zone;

        if (time.Hour < WORKING_DAY_END_HOUR && time.Hour >= WORKING_DAY_START_HOUR)
        {
            // if we are in the middle of working day, then:
            // - start date is 9:00 of current day
            // - end date is 18:00 of current day
            //
            return (timezone.AtStartOfDay(time.Date).PlusHours(WORKING_DAY_START_HOUR), timezone.AtStartOfDay(time.Date).PlusHours(WORKING_DAY_END_HOUR));
        }

        if (time.Hour > WORKING_DAY_END_HOUR)
        {
            // if working day is over, but it's still this day (till 00:00), then:
            // - start date is 9:00 of next day
            // - end date is 18:00 of next day
            //
            return (timezone.AtStartOfDay(time.Date.PlusDays(1)).PlusHours(WORKING_DAY_START_HOUR), timezone.AtStartOfDay(time.Date.PlusDays(1)).PlusHours(WORKING_DAY_END_HOUR));
        }

        if (time.Hour < WORKING_DAY_START_HOUR)
        {
            // if working day is over, but it's new day already (after 00:00), then:
            // - start date is 9:00 of current day
            // - end date is 18:00 of current day
            //
            return (timezone.AtStartOfDay(time.Date).PlusHours(WORKING_DAY_START_HOUR), timezone.AtStartOfDay(time.Date).PlusHours(WORKING_DAY_END_HOUR));
        }

        return (time, time);
    }

    private string GetFormattedAppointmentsReplyMessage(IEnumerable<OutlookAppointmentDto> appointments, ZonedDateTime time)
    {
        var sb = new StringBuilder($"*Социальность рейтинг поднятие {time:dd/MM/yyyy}:*".Escape());
        
        if (appointments.Any())
        {
            sb.AppendLine();
            sb.AppendLine();

            foreach (var app in appointments)
            {
                sb.AppendLine($"{Instant.FromDateTimeUtc(app.Start.ToUniversalTime()).InZone(time.Zone):HH:mm}-{Instant.FromDateTimeUtc(app.End.ToUniversalTime()).InZone(time.Zone):HH:mm}".Escape() + 
                    $"    [{app.Subject.Escape()}]({app.Location.Escape()})");
            }
        }

        sb.AppendLine();
        sb.AppendLine($"+{appointments.Count() * 10} социальный рейтинг 红龙习近平.".Escape());

        return sb.ToString();
    }
}
