using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Models
{
    public class CustomerModel
    {
        public string CustomerId { get; set; }

        public string Remarks { get; set; }
        public string AspNetUsersId { get; set; }

        //public string Address { get; set; }

        //public string NearestBustop { get; set; }

        //public string Photo { get; set; }

        //[Required, Display(Name = "Mobile Number 1")]
        //public string FirstMobile { get; set; }

        //[Display(Name = "Mobile Number 2")]
        //public string SecondMobile { get; set; }

        //public string Email { get; set; }
        //[Display(Name = "Active")]
        //public bool IsActive { get; set; }

        //public bool IsDeleted { get; set; }

        //public DateTime DateCreated { get; set; }

        //public virtual BranchModel Branches { get; set; }
        public virtual ApplicationUser AspNetUsers { get; set; }
        public virtual ICollection<DepositModel> Deposits { get; set; }
        public virtual ICollection<LoanModel> Loans { get; set; }
    }
}