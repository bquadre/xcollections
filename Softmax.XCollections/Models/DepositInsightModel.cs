using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class DepositInsightModel
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string Balance { get; set; }
        public string LastDeposit { get; set; }
        public string LastWithdrawal { get; set; }
        public string LastDepositAmount { get; set; }
        public string LastWithdrawalAmount { get; set; }
        public int Deposits { get; set; }
        public int Withdrawals { get; set; }
        public string TotalDeposits { get; set; }
        public string TotalWithdrawals { get; set; }
        public string TotalFees { get; set; }
       
    }
}
