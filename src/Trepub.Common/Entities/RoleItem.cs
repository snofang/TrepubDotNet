using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public partial class RoleItem
    {
        public RoleItem()
        {
            ProfileRole = new HashSet<ProfileRole>();
            RoleItemResource = new HashSet<RoleItemResource>();
        }

        [Key]
        public int RoleItemId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
		[Key]
        [IgnoreUpdate]
        public DateTime LastModifiedTime { get; set; }

        public ICollection<ProfileRole> ProfileRole { get; set; }
        public ICollection<RoleItemResource> RoleItemResource { get; set; }
    }
}
