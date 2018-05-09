using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Models
{
    public class BranchModel
    {
        public string BranchId { get; set; }

        public string BranchCode { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required, Display(Name="State")]
        public StateCode StateCode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }  
    }
}