namespace GreatLeaderXiBot.Domain.Core.Contracts
{
    using Microsoft.Exchange.WebServices.Data;

    public interface IOutlookConnector
    {
        List<Appointment> GetAppointments(DateTime dateFrom, DateTime dateTo);
    }
}
