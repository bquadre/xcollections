using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Models
{
    public class LoanModel
    {
        public string LoanId { get; set; }

        [Required, Display(Name = "Account Number")]
        public string CustomerId { get; set; }

        public string EmployeeId { get; set; }


        public string Reference { get; set; }

        [Required, Display(Name = "Loans Products")]
        public string ProductId { get; set; }

        [Required]
        public int Amount { get; set; }

        public int Interest { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DueDate { get; set; }

       
        public DateTime DateCreated { get; set; }

        public DateTime? DateApproved { get; set; }

        public StatusCode StatusCode { get; set; } 


        public string Remarks { get; set; }

        public virtual CustomerModel Customer { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual ProductModel Product { get; set; }
        public virtual ICollection<RefundModel> Refunds { get; set; }
        //public virtual ICollection<GuarantorModel> Guarantors { get; set; }

    }
}