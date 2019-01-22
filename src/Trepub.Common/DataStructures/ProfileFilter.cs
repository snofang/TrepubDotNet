using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.DataStructures
{
    public class ProfileFilter
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }
        public int? RoleItemId { get; set; }

    }
}
