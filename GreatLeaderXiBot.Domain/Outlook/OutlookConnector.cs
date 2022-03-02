namespace GreatLeaderXiBot.Domain.Outlook
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.Exchange.WebServices.Data;

    using Core.Contracts;

    public class OutlookConnector : IOutlookConnector
    {
        #region Fields

        private readonly ExchangeService _exchangeService;

        #endregion

        #region Constructors

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

        #endregion

        #region Methods

        public List<Appointment> GetAppointments(DateTime dateFrom, DateTime dateTo)
        {
            var calendar = CalendarFolder.Bind(_exchangeService, WellKnownFolderName.Calendar, new PropertySet());
            var cView = new CalendarView(dateFrom, dateTo)
            {
                PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End)
            };

            try
            {
                return calendar.FindAppointments(cView).ToList();
            }
            catch
            {
                return new List<Appointment>();
            }
        }

        static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }
}
