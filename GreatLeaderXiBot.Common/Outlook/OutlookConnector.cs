namespace GreatLeaderXiBot.Common.Outlook;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Exchange.WebServices.Data;

using Dtos;

public class OutlookConnector : IOutlookConnector
{
    private readonly ExchangeService _exchangeService;

    public OutlookConnector(string exchangeHost, string exchangeLogin, string exchangePassword)
    {
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

        _exchangeService = new ExchangeService()
        {
            KeepAlive = true,
            Credentials = new WebCredentials(exchangeLogin, exchangePassword),
            Url = new Uri(exchangeHost)
        };
    }

    public async Task<IEnumerable<OutlookAppointmentDto>> GetAppointmentsAsync(DateTime dateFrom, DateTime dateTo)
    {
        var calendar = await CalendarFolder.Bind(_exchangeService, WellKnownFolderName.Calendar, new PropertySet());
        var cView = new CalendarView(dateFrom, dateTo)
        {
            PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End)
        };

        try
        {
            var appointments = await calendar.FindAppointments(cView);
            return appointments.Select(x => new OutlookAppointmentDto { Subject = x.Subject, Start = x.Start, End = x.End });
        }
        catch
        {
            return new List<OutlookAppointmentDto>();
        }
    }

    static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
