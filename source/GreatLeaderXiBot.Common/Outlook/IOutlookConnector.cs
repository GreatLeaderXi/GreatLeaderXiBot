namespace GreatLeaderXiBot.Common.Outlook;

using NodaTime;
using Dtos;

public record OutlookSettings(string Host, string Login, string Password);

public interface IOutlookConnector
{
    Task<IEnumerable<OutlookAppointmentDto>> GetAppointmentsAsync(ZonedDateTime dateFrom, ZonedDateTime dateTo, OutlookSettings settings);
}
