using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models.Messages
{
    public class SmtpModel
    {
        public string Provider { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public string Port { get; set; }
        public bool IsSecure { get; set; }
    }
}
