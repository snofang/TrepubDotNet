using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Trepub.Common;
using Trepub.Common.Entities;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using Trepub.Web.API.ViewModels;
using Trepub.Web.API.ViewModels.Profile;
using System.Collections.Generic;
using Trepub.Web.API.Extensions;
using Trepub.Web.API.ViewModels.Common;
using Trepub.BSO;
using Trepub.Common.EntitiesExt;

namespace Trepub.Web.API.Controllers
{
    [Route("api/[controller]")]
    public class ProfilesController : Controller
    {
        protected RoleBSO partyRoleBSO;
        protected IExternalServices externalServices;
        protected IConfiguration configuration;

        public ProfilesController()
        {
            this.partyRoleBSO = BusinessFacade.GetBSO<RoleBSO>();
            this.externalServices = FacadeProvider.IfsFacade.GetExternalServices();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
        }

        [HttpPost("createNormal")]
        [AllowAnonymous]
        public ProfileView CreateProfile([FromBody] ProfileView profileView)
        {
            Profile p = profileView.ToProfile();
            p = BusinessFacade.GetBSO<ProfileBSO>().CreateNormalProfile(p);
            return p.ToProfileView();
        }

        [HttpPost("updateMine")]
        public ProfileView UpdateProfileMine([FromBody] ProfileView profileView)
        {
            Profile p = profileView.ToProfile();
            p.ProfileId = this.GetCurrentUserId();
            p = BusinessFacade.GetBSO<ProfileBSO>().UpdateProfile(p);
            return p.ToProfileView();
        }

        [HttpGet("getMine")]
        public IActionResult GetProfile()
        {
            int profileId = this.GetCurrentUserId();
            var p = BusinessFacade.GetBSO<ProfileBSO>().GetProfile(new Profile() { ProfileId = profileId});
            var profileView = p.ToProfileView();
            return new ObjectResult(profileView);
        }

        [HttpPost("resetPasswordMine")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody]ProfileView profileView)
        {
            var profile = profileView.ToProfile();
            BusinessFacade.GetBSO<ProfileBSO>().ResetPassword(profile);
            return new ObjectResult(null);
        }

        [HttpPost("list")]
        public DataListView<ProfileListingView> GetProfiles([FromBody] ProfileFilterView filter)
        {
            var list = BusinessFacade.GetBSO<ProfileBSO>().GetProfiles(filter, filter.Page, filter.PageSize);
            var totalCount = BusinessFacade.GetBSO<ProfileBSO>().GetProfilesTotalCount(filter);
            var result = new DataListView<ProfileListingView>()
            {
                DataList = GetProfileListingView(list),
                TotalCount = totalCount
            };
            return result;
        }

        [HttpPost("changePasswordMine")]
        public IActionResult ChangeMyPassword([FromBody] ChangePasswordView cpv)
        {
            BusinessFacade.GetBSO<ProfileBSO>().ChangePassword(this.GetCurrentUserId(), cpv.OldPassword, cpv.NewPassword);
            return new ObjectResult(null);
        }

        [HttpDelete("deleteMine")]
        public IActionResult DeleteProfile()
        {
            int profileId = this.GetCurrentUserId();
            BusinessFacade.GetBSO<ProfileBSO>().DeactivateProfile(profileId);
            return new ObjectResult(null);
        }

        //[HttpPost("sendcode")]
        //public IActionResult SendVerificationCode()
        //{
        //    int profileId = this.GetCurrentUserId();
        //    var profile = BusinessFacade.GetBSO<PartyProfileBSO>().SendVerificationCode(new Profile() { ProfileId = profileId });
        //    return new ObjectResult(profile.ToProfileView());
        //}

        private IEnumerable<ProfileListingView> GetProfileListingView(List<ProfileExt> list)
        {
            List<ProfileListingView> result = new List<ProfileListingView>();
            foreach (var p in list)
            {
                var profileLV = new ProfileListingView()
                {
                    ProfileId = p.ProfileId,
                    UserId = p.UserId,
                    Name = p.Name,
                    LastName = p.LastName,
                    MobileNumber = p.MobileNumber,
                    Status = p.Status,
                };
                result.Add(profileLV);
            }
            return result;
        }

    }
}
