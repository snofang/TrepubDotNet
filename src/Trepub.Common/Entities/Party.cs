using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public class Party
    {

        [Key]
        public int PartyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string ExternalId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BankAccId { get; set; }
        public string Description { get; set; }

    }
}
