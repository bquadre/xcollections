using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Data.Entities
{
    public class Loan
    {
        [Key]
        public string LoanId { get; set; }
        public string CustomerId { get; set; }
        public string EmployeeId { get; set; }
        public string ProductId { get; set; }
        public string Reference { get; set; }
        public int Amount { get; set; }
        public int Interest { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DueDate { get; set; }      
        public DateTime? DateCreated { get; set; }
        public DateTime? DateApproved { get; set; }
        public StatusCode StatusCode { get; set; }
        public string Remarks { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
    }
}