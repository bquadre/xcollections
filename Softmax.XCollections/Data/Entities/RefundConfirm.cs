using System;

namespace Softmax.XCollections.Data.Entities
{
    public class RefundConfirm
    {
        public string RefundConfirmId { get; set; }
        public string RefundId { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }

        public virtual Refund Refund { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
