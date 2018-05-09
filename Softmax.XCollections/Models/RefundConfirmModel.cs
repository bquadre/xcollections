using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Models
{
    public class RefundConfirmModel
    {
        public string RefundConfirmId { get; set; }
        public string RefundId { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }

        public virtual RefundModel Refund { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual CustomerModel Customer { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
