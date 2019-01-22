using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Trepub.Web.API
{
    public class AppAuthorizeFilter : AuthorizeFilter
    {
        public AppAuthorizeFilter(AuthorizationPolicy policy) : base(policy)
        {

        }
    }
}
