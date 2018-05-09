using System;

namespace Softmax.XCollections.Models
{
    public class MessageModel
    {
        public string MessageId { get; set; }

        public string EmployeeId { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        //public virtual EmployeeModel Employee { get; set; }

    }
}