using Microsoft.Extensions.Options;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Models;
using Softmax.XCollections.Models.Messages;
using Softmax.XCollections.Models.Settings;

namespace Softmax.XCollections.Utilities
{

    public class Messager : IMessager
    {

        private readonly SmtpSettingsModel _smtpSettings;

        public Messager(IOptions<SmtpSettingsModel> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        
        public void SendEmailResetPassword(string email, string tempPassword)
        {

            var message = new SimpleEmailModel
            {
                From = "SBC Nigeria",
                To = email,
                Subject = "Request password reset",
                Body = "Dear " + email + ","
            };
            message.Body += "<br/><br/>";
            message.Body += "You recently requested for password reset on SBC website below is a temp password";
            message.Body += "<br/><br/>";
            message.Body += tempPassword;
            message.Body += "<br/><br/>";
            message.Body += "Thank You.";
            message.Body += "<br/><br/>";
            message.Body += "Regards,<br/>";
            message.Body += "SBC Team";

            SmtpMail.Send(message, _smtpSettings);

        }
    }
}
