using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public partial class Idmap
    {
        [Key]
        public string Idkey { get; set; }
        public int Idvalue { get; set; }
    }
}
