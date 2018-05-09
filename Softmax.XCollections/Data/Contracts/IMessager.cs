using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Data.Contracts
{
    public interface IMessager
    {
        void SendEmailResetPassword(string email, string tempPassword);
    }
}
