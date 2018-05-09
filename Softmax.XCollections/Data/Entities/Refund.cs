using System;
using System.ComponentModel.DataAnnotations;
using Softmax.XCollections.Data.Enums;

namespace Softmax.XCollections.Data.Entities
{
    public class Refund
    {
        [Key]
        public string RefundId { get; set; }
        public string CustomerId { get; set; }
        public string EmployeeId { get; set; }
        public string LoanId { get; set; }
        public string Reference { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public StatusCode StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Loan Loan { get; set; }
    }
}