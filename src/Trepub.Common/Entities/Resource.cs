using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public partial class Resource
    {
        public Resource()
        {
            RoleItemResource = new HashSet<RoleItemResource>();
        }

        [Key]
        public int ResourceId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ResourceContent { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
		[Key]
        [IgnoreUpdate]
        public DateTime LastModifiedTime { get; set; }

        public ICollection<RoleItemResource> RoleItemResource { get; set; }
    }
}
