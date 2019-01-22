using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Trepub.Web.API.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API
{
    public class ValidationFilter : IActionFilter
    {
        private bool processRequests = true;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //do nothing
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.HttpContext.Request.Path.Equals("/api/system/toggleProcessingRequestsMode"))
            {
                processRequests = !processRequests;
                context.Result = new ObjectResult(new SingleValueView() { Value = processRequests.ToString()});
            }
            else
            {
                if (!processRequests)
                {
                    context.Result = new ObjectResult("Trepub is on maintenace, please wait!");
                };
            }
        }
    }
}
