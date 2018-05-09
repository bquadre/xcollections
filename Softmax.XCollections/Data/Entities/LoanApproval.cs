using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Data.Entities
{
    public class LoanApproval
    {
        [Key]
        public string LoanApprovalId { get; set; }
        public string LoanId { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }

        public virtual Loan Loan { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
