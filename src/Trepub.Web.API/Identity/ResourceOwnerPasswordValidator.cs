using IdentityModel;
using IdentityServer4.Validation;
using Trepub.Common;
using Trepub.Common.Extensions;
using System.Threading.Tasks;
using Trepub.BSO;

namespace Trepub.Web.API.Identity
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {

            var profile = BusinessFacade.GetBSO<ProfileBSO>().GetProfileSlim(new Common.Entities.Profile() { UserId = context.UserName } );
            if(profile != null && profile.Password.Equals(context.Password.HashPassword()))
            {
                context.Result =
                    new GrantValidationResult(profile.ProfileId.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
            return Task.FromResult(0);
        }
    }
}
