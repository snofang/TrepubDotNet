using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Trepub.Common;
using Trepub.Common.Facade;
using Trepub.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trepub.BSO;

namespace Trepub.Web.API
{
    public class RoleResourceRequirementHandler : AuthorizationHandler<RoleResourceRequirement>
    {
        protected ILogger<RoleResourceRequirementHandler> logger;
        protected RoleBSO partyRoleBSO;
        protected IConfiguration configuration;


        public RoleResourceRequirementHandler() : base()
        {
            this.logger = FacadeProvider.IfsFacade.GetLogger<RoleResourceRequirementHandler>();
            this.partyRoleBSO = BusinessFacade.GetBSO<RoleBSO>();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleResourceRequirement requirement)
        {
            if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext)
            {
                try
                {
                    var subjectClaims = context.User.Claims.Where(c => c.Type.Equals("sub"));
                    var path = mvcContext.HttpContext.Request.Path.Value;
                    if (subjectClaims.Count() > 0)
                    {
                        var profileId = int.Parse(subjectClaims.First().Value);
                        if (partyRoleBSO.HasPermission(profileId, path))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            logger.LogWarning($"Authorization: permission denied; profileId={profileId}, path={path}");
                        }
                    }
                    else
                    {
                        //TODO: to provide userId checking based on db stored Ids; e.g. remove this harcoded id.
                        var clientIdClaim = context.User.Claims.First(c => c.Type.Equals("client_id"));
                        if (clientIdClaim.Value.Equals("AzartelMASClient"))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            logger.LogWarning($"Authorization: permission denied; clientId={clientIdClaim.Value}, path={path}");
                        }
                    }
                }
                catch (Exception exc)
                {
                    context.Fail();
                    logger.LogError("Authorization Error" + exc.ToString(), exc);
                }

            }
            return Task.CompletedTask;
        }
    }
}
