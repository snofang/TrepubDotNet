using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public partial class ProfileRole
    {
        [Key]
        public int RoleItemId { get; set; }
        [Key]
        public int ProfileId { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
		[Key]
        [IgnoreUpdate]
        public DateTime LastModifiedTime { get; set; }

        public Profile Profile { get; set; }
        public RoleItem RoleItem { get; set; }
    }
}
