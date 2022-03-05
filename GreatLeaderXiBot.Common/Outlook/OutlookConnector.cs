namespace GreatLeaderXiBot.Common.Outlook;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Exchange.WebServices.Data;

using Dtos;

public class OutlookConnector : IOutlookConnector
{
    public OutlookConnector()
    {
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
    }

    public async Task<IEnumerable<OutlookAppointmentDto>> GetAppointmentsAsync(DateTime dateFrom, DateTime dateTo, OutlookSettings settings)
    {
        var exchangeService = new ExchangeService()
        {
            KeepAlive = true,
            Credentials = new WebCredentials(settings.Login, settings.Password),
            Url = new Uri(settings.Host)
        };

        var calendar = await CalendarFolder.Bind(exchangeService, WellKnownFolderName.Calendar, new PropertySet());
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
