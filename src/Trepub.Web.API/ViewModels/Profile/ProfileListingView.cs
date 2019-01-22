using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels.Profile
{
    public class ProfileListingView
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }


    }
}
