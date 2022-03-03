namespace GreatLeaderXiBot.Domain.Core.Contracts
{
    using Microsoft.Exchange.WebServices.Data;

    public interface IOutlookConnector
    {
        Task<List<Appointment>> GetAppointmentsAsync(DateTime dateFrom, DateTime dateTo);
    }
}
