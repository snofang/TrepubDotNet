using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels.Profile
{
    public class ProfileView
    {
        public int? ProfileId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }
        public string VerificationCode { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

    }
}
