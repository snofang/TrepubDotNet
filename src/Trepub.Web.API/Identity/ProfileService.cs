using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Trepub.Common;
using IdentityServer4.Extensions;
using System.Security.Claims;
using IdentityModel;
using Trepub.BSO;

namespace Trepub.Web.API.Identity
{
    public class ProfileService : IProfileService
    {


        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            var profileId = int.Parse(subjectId);
            var profile = BusinessFacade.GetBSO<ProfileBSO>().GetProfileSlim(new Common.Entities.Profile() { ProfileId = profileId });

            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject, profile.ProfileId.ToString()),
                new Claim(JwtClaimTypes.Id, profile.UserId)
            };
            if (!String.IsNullOrEmpty(profile.UserId))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, profile.UserId));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, profile.UserId));
            }

            context.IssuedClaims = claims;
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            if (!subjectId.IsNullOrEmpty())
            {
                var profileId = int.Parse(subjectId);
                var profile = BusinessFacade.GetBSO<ProfileBSO>().GetProfileSlim(new Common.Entities.Profile() { ProfileId = profileId });
                context.IsActive = profile != null;
            }
            else
            {
                context.IsActive = false;
            }
            return Task.FromResult(0);
        }

    }
}
