using System.Collections.Generic;

namespace Softmax.XCollections.Models
{
    public class CustomerViewModel
    {
        public CustomerModel CustomerInfo { get; set; }
        public BranchModel Branch { get; set; }
        public List<DepositModel> Deposits { get; set; }
        public List<WithdrawalModel> Withdrawals { get; set; }
        public List<LoanModel> Loans { get; set; }      
    }
}