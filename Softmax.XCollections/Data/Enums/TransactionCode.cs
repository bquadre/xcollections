using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softmax.XCollections.Data.Enums
{
    public enum TransactionCode
    {
        None =0,
        Deposit =1,
        Withdrawal =2,
        LoanRequest = 3,
        LoanRefund = 4,
        Fee = 5
    }
}