using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Lotereya.Helpers
{
    public static class CoreHelper
    {
        public static string SiteRootUrl(string item)
        {
            string domain = "http://free-online-lottery.com";
            string result = string.Format("{0}/{1}", domain, item.TrimStart('/'));
            return result;
        }

        public static void SendMail(string mailFrom, string displayName, string mailTo, string copyMail, string subject, string body)
        {
            using (MailMessage msg = new MailMessage())
            {
                if (mailFrom == null) mailFrom = "akon@" + HttpContext.Current.Request.Url.Host;

                if (mailTo == null) mailTo = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];

                if (displayName == null) displayName = "AKon CMS";

                msg.From = new MailAddress(mailFrom, displayName);
                msg.To.Add(new MailAddress(mailTo));

                if (copyMail != null)
                    msg.CC.Add(new MailAddress(copyMail));
                msg.Subject = subject;
                msg.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");

                msg.IsBodyHtml = true;
                msg.Body = body;

                var smtpNetwork = ((SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp")).Network;
                SmtpClient smtpServer = new SmtpClient(smtpNetwork.Host);
                smtpServer.Credentials = new System.Net.NetworkCredential(smtpNetwork.UserName, smtpNetwork.Password);
                smtpServer.Port = smtpNetwork.Port; // Gmail works on this port
                smtpServer.EnableSsl = smtpNetwork.EnableSsl; //для отправки через гмаил

                smtpServer.Send(msg);
            }
        }
    }
}