using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Trepub.Common.Entities
{
    public class Price
    {
        [Key]
        public int PriceId { get; set; }
        public string TypeCode { get; set; }
        public decimal PriceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Status { get; set; }
        public decimal ScoreRatio { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get;set;}

    }
}
