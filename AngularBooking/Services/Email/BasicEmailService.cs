using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AngularBooking.Services.Email
{
    public class BasicEmailService : IEmailService
    {
        // reference used for global testing purposes
        public static string TestPickupDirectory = null;

        private SmtpClient _client;
        private string _defaultFrom = "info@angularbookingdemo.com";
        
        public BasicEmailService(string testPickupDirectory, string configString)
        {
            TestPickupDirectory = testPickupDirectory;

            _client = new SmtpClient();

            // extract config details
            string[] configData = configString != null ? configString.Split(";") : new string[0];

            // if directory specified, or incorrect number of smtp parameters are specified, use local test directory for pickup
            if (testPickupDirectory != null || configData.Length < 4)
            {
                _client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                _client.PickupDirectoryLocation = testPickupDirectory;
                _client.DeliveryFormat = SmtpDeliveryFormat.International;
            }
            else
            {
                _client.Host = configData[0];
                _client.Port = int.Parse(configData[1]);
                _client.Credentials = new NetworkCredential(configData[2], configData[3]);
                if (configData.Length > 4)
                    _defaultFrom = configData[4];
            }
        }

        public bool SendEmail(string to, string subject, string content, string from = null)
        {
            // if null, use default
            if (from == null)
                from = _defaultFrom;

            try
            {
                // attempt send
                MailMessage message = new MailMessage(from, to, subject, content);
                message.BodyTransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                _client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                // catch-all, fail silently and return false
                return false;
            }
        }
    }
}
