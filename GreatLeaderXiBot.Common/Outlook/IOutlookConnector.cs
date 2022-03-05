namespace GreatLeaderXiBot.Common.Outlook;

using Dtos;

public record OutlookSettings(string Host, string Login, string Password);

public interface IOutlookConnector
{
    Task<IEnumerable<OutlookAppointmentDto>> GetAppointmentsAsync(DateTime dateFrom, DateTime dateTo, OutlookSettings settings);
}
