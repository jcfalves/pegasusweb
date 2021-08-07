using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.ApiClient.Helpers
{
    public class EmailHelper
    {

        public static string GetTemplateEmail(string folder, string nomeTemplate) {

            var pathTemplate = System.IO.Path.Combine(folder, "email", nomeTemplate + ".html");
            var data = System.IO.File.ReadAllText(pathTemplate);

            data = data.Replace("#HOSTNAME#", Bayer.Pegasus.Utils.Configuration.Instance.AppDomainURL);

            data = data.Replace("#URLPREFIX#", Bayer.Pegasus.Utils.Configuration.Instance.URLPrefix);

            return data;
        }

        public static void SendEmail(string mailTo, string subject, string body, bool htmlMail)
        {

            var configuration = new EmailService.emailSoapClient.EndpointConfiguration();
           
            var url = Bayer.Pegasus.Utils.Configuration.Instance.ServiceEmailURL;

            var service = new EmailService.emailSoapClient(configuration, url);

            service.sendMailAsync(mailTo, subject, body, htmlMail);

        }

        public static void SendErrorEmail(string subject, string body)
        {
            string mailTo = Utils.Configuration.Instance.ErrorMailTo;

            bool htmlMail = true;

            SendEmail(mailTo, subject, body, htmlMail);

        }

    }
}
