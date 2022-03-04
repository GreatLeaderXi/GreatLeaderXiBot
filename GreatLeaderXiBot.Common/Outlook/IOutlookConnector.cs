namespace GreatLeaderXiBot.Common.Outlook;

using Dtos;

public interface IOutlookConnector
{
    Task<IEnumerable<OutlookAppointmentDto>> GetAppointmentsAsync(DateTime dateFrom, DateTime dateTo);
}
