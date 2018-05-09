using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class LoanApprovalModel
    {
        public string LoanApprovalId { get; set; }
        public string LoanId { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }

        public virtual LoanModel Loan { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual CustomerModel Customer { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
