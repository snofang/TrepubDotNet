using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Trepub.Common.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreditDebit { get; set; }
        public int BeholderId { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int AccountId { get; set; }

    }
}
