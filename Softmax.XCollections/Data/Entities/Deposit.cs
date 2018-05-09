using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Data.Entities
{
    public class Deposit
    {
       [Key]
        public string DepositId { get; set; }
        public string CustomerId { get; set; }
        public string EmployeeId { get; set; }
        public string ProductId { get; set; }
        public string Reference { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public TransactionCode TransactionCode { get; set; }
        public StatusCode StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Product Product { get; set; }
    }
}