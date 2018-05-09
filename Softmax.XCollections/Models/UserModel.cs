using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Softmax.XCollections.Data.Entities;

namespace Softmax.XCollections.Models
{
    public class UserModel
    {
        [Required, Display(Name = "User Type")]
        public UserCode UserCode { get; set; }

        [Display(Name = "Branch")]
        public string BranchId { get; set; }
        
        public string Id { get; set; }

        [Display(Name = "Occupation/Designation")]
        public string Occupation { get; set; }

        public string UniqueNumber { get; set; }

        [Required, Display(Name="Gender")]
        public GenderCode GenderCode { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Other Name")]
        public string OtherName { get; set; }

        [Required, Display(Name = "Mobile Number1"), MaxLength(13)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Mobile Number2"), MaxLength(13)]
        public string MobileNumber { get; set; }

        public string Email { get; set; }

        [Display(Name = "Contact Address")]
        public string Address { get; set; }

        [Display(Name = "Nearest Bus Stop")]
        public string NearestBusStop { get; set; }

        public string Photo { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        //public virtual ApplicationUser AspNetUsers { get; set; }
        public virtual BranchModel Branch { get; set; }
        public virtual ICollection<DepositModel> Deposits { get; set; }
        public virtual ICollection<LoanModel> Loans { get; set; }
    }
}