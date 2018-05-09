using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Data.Entities
{
    public class Employee
    {
        [Key]
        public string EmployeeId { get; set; }
        public string AspNetUsersId { get; set; }
        public DateTime? Employment { get; set; }
        public DateTime? Termination { get; set; }

        public virtual ApplicationUser AspNetUsers { get; set; }
        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}