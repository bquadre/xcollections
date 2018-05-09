using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Data.Enums;

namespace Softmax.XCollections.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string BranchId { get; set; }
        public string UniqueNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string MobileNumber { get; set; }
        public GenderCode GenderCode { get; set; }
        public UserCode UserCode { get; set; }
        public string Occupation { get; set; }
        public string Address { get; set; }
        public string NearestBusStop { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
        public bool IsTempPassword { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}
