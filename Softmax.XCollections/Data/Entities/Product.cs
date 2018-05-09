using Softmax.XCollections.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Softmax.XCollections.Data.Entities
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; }
        public ProductCode ProductCode { get; set; }
        public string Name { get; set; }
        public int Tenure { get; set; } = 0;//days
        public int Rate { get; set; } = 0;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }  
    }
}