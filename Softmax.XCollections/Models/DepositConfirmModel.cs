using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class DepositConfirmModel
    {
        public string DepositConfirmId { get; set; }
        public string DepositId { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }

        public virtual DepositModel Deposit { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual CustomerModel Customer { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
