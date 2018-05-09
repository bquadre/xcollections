using System;
using System.ComponentModel.DataAnnotations;
using Softmax.XCollections.Data.Enums;

namespace Softmax.XCollections.Models
{
    public class RefundModel
    {
        public string RefundId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public string EmployeeId { get; set; }
   
        public string LoanId { get; set; }

        public string Reference { get; set; }

        [Required]
        public int Amount { get; set; }

        public int Balance { get; set; }

        public StatusCode StatusCode { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        //public virtual CustomerModel Customer { get; set; }
        //public virtual EmployeeModel Employee { get; set; }
        public virtual LoanModel Loan { get; set; }
    }
}