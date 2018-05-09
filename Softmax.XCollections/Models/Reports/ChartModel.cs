using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class ChartModel
    {
        public List<string> Dates { get; set; }
        public List<int> Deposits { get; set; }
        public List<int> Withdrawals { get; set; }
        public List<int> Refunds { get; set; }
        public List<int> Incomes { get; set; }
    }
}
