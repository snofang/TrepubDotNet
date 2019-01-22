using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels
{
    public class ChangePasswordView
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
