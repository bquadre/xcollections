using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Softmax.XCollections.Models
{
    public class DepositModel
    {
        public string DepositId { get; set; }
        [Required, Display(Name="Account Number")]
        public string CustomerId { get; set; }

        public string EmployeeId { get; set; }
        [Required, Display(Name="Savings Products")]
        public string ProductId { get; set; }

        public string Reference { get; set; }
        [Required, Display(Name = "Transaction Type")]
        public TransactionCode TransactionCode { get; set; }
        public StatusCode StatusCode { get; set; }

        [Required]
        public int Amount { get; set; }

        public int Balance { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual CustomerModel Customer { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual ProductModel Product { get; set; }

    }
}