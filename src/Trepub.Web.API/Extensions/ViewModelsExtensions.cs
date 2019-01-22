using Trepub.Common.Entities;
using Trepub.Common.Facade;
using Trepub.Web.API.ViewModels;
using Trepub.Web.API.ViewModels.Profile;

namespace Trepub.Web.API.Extensions
{
    public static class ViewModelsExtensions
    {

        public static ProfileView ToProfileView(this Profile profile)
        {
            ProfileView result = null;
            if (profile != null)
            {
                bool testing = false;
                bool.TryParse(FacadeProvider.IfsFacade.GetConfigurationRoot().GetSection("LocalSettings")["TestEnvironment"], out testing);
                result = new ProfileView()
                {
                    MobileNumber = profile.MobileNumber,
                    Status = profile.Status,
                    VerificationCode = testing ? profile.VerificationCode : string.Empty,
                    ProfileId = profile.ProfileId,
                    UserId = profile.UserId,
                    Name = profile.Party.Name,
                    LastName = profile.Party.LastName,
                    Phone = profile.Party.Phone,
                    Address = profile.Party.Address,
                };
            }
            return result;
        }

        public static Profile ToProfile(this ProfileView pv)
        {
            //profile
            Profile p = new Profile()
            {
                Password = pv.Password,
                UserId = pv.UserId,
                VerificationCode = pv.VerificationCode
            };
            p.ProfileId = pv.ProfileId.HasValue ? pv.ProfileId.Value : 0;

            //party
            p.Party = new Party()
            {
                Name = pv.Name,
                LastName = pv.LastName,
                Phone = pv.Phone,
                Address = pv.Address,
            };
            return p;
        }


    }
}
