using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Trepub.Common;
using Trepub.Common.Exceptions;
using System;
using System.Linq;
using System.Security.Claims;

namespace Trepub.Web.API.Controllers
{
    public static class ControllerExtensions
    {

        public static bool AnyUserLogedin(this Controller controller)
        {
            var caller = controller.User as ClaimsPrincipal;
            String subjectId = caller.Claims.First(c => c.Type.Equals("sub")).Value;
            return !string.IsNullOrEmpty(subjectId);
        }

        public static int GetCurrentUserId(this Controller controller)
        {
            var caller = controller.User as ClaimsPrincipal;
            String subjectId = caller.Claims.First(c => c.Type.Equals("sub")).Value;
            return int.Parse(subjectId);
        }


    }
}
