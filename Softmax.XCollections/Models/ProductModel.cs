using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Models
{
    public class ProductModel
    {
        public string ProductId { get; set; }
        [Required]
        [Display(Name="Type")]
        public ProductCode ProductCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Tenure { get; set; } = 0;//weeks

        [Required]
        public int Rate { get; set; } = 0;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

    }
}