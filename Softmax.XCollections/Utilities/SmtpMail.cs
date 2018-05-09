using System;
using System.Net.Mail;
using Softmax.XCollections.Models.Messages;
using Softmax.XCollections.Models.Settings;

namespace Softmax.XCollections.Utilities
{
    public static class SmtpMail
    {
        public static bool Send(SimpleEmailModel message, SmtpSettingsModel smtp)
        {

            var mail = new MailMessage(smtp.Sender, smtp.Sender);
            var client = new SmtpClient
            {
                Port = int.Parse(smtp.Port),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(smtp.Username, smtp.Password),
                Host = smtp.Provider
            };
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.To.Add(message.To);
            mail.IsBodyHtml = true;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}
