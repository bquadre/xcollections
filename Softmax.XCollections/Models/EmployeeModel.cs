using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Models
{
    public class EmployeeModel
    {
        public string EmployeeId { get; set; }
        public string AspNetUsersId { get; set; }
        //public string Id { get; set; }

        //[Display(Name = "Designation")]
        //public string DesignationId { get; set; }

        //public string EmployeeNumber { get; set; }

        //[Required, Display(Name="Gender")]
        //public GenderCode GenderCode { get; set; }

        //[Required, Display(Name = "First Name")]
        //public string FirstName { get; set; }

        //[Required, Display(Name = "Last Name")]
        //public string LastName { get; set; }

        //[Required, MaxLength(13)]
        //public string Mobile { get; set; }

        //public string Email { get; set; }

        //[Required, Display(Name = "Active")]
        //public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }
        // public DateTime DateCreated { get; set; }
        public DateTime? Employment { get; set; }
        public DateTime? Termination { get; set; }

        //public virtual BranchModel Branches { get; set; }
        public virtual ApplicationUser AspNetUsers { get; set; }
        public virtual ICollection<DepositModel> Deposits { get; set; }
        public virtual ICollection<LoanModel> Loans { get; set; }
       // public virtual ICollection<RefundModel> Refunds { get; set; }
    }
}