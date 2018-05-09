using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models
{
    public class GroupedDepositModel
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public int Balance { get; set; }
        public int Count { get; set; }
        public string UniqueNumber { get; set; }
    }
}
