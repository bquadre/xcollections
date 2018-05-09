using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Data.Entities
{
    public class Branch
    {
        [Key]
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public StateCode StateCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
    }
}