using Softmax.XCollections.Data.Enums;
using System;

namespace Softmax.XCollections.Models
{
    public class WithdrawalModel
    {
        public string WithdrawalId { get; set; }

        public string CustomerId { get; set; }

        public string EmployeeId { get; set; }//AccountOfficer

        public string Reference { get; set; }

        public int Amount { get; set; }

        public StatusCode StatusCode { get; set; }

        public string Status { get; set; }

        public DateTime WhenRequested { get; set; }

        public DateTime? WhenTreated { get; set; }

        public bool IsDeleted { get; set; }

        public string Remark { get; set; }

    }
}