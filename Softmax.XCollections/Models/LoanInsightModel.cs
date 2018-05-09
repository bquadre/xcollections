using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class LoanInsightModel
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string TotalLoans { get; set; }
        public string TotalRefunds { get; set; }
        public string LastLoan { get; set; }
        public string LastRefund { get; set; }
        public string LastLoanAmount { get; set; }
        public string LastRefundAmount { get; set; }
        public int? Loans { get; set; }
        public int? Refunds { get; set; }
    }
}
