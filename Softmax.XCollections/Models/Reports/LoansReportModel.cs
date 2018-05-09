using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models.Reports
{
    public class LoansReportModel
    {
        public string Date { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Disbursed { get; set; }
        public string Defaulted { get; set; }
        public string AmountDisbursed { get; set; }
        public string ExpectedRefund { get; set; }
        public string ExpectedProfit { get; set; }
    }
}
