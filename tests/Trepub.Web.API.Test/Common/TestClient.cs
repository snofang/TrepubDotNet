using IdentityModel;
using IdentityModel.Client;
using Trepub.Web.API.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using Trepub.Web.API.ViewModels.Profile;

namespace Trepub.Web.API.Test.Common
{
    public class TestClient : BaseAppClient
    {

        protected ProfileView profile;

        public ProfileView Profile
        {
            get
            {
                return this.profile;
            }
        }

        public TestClient() : this(null, null) { }

        public TestClient(TimeSpan timeOut) : this(null, null, timeOut) { }

        public TestClient(string baseUrl, ProfileView profile) : this(baseUrl, profile, new TimeSpan()) { }

        public TestClient(ProfileView profile) : this(null, profile, new TimeSpan()) { }

        public TestClient(string baseUrl, ProfileView profile, TimeSpan timespan) : base(baseUrl, timespan)
        {
            //profile
            if (profile == null)
            {
                this.profile = new ProfileView()
                {
                    Password = "Trepub123",
                    UserId = "09123456789"
                };

            }
            else
            {
                this.profile = profile;
            }

        }

        public void Login()
        {
            var tokenClient = new TokenClient(baseUrl + "/connect/token" /*endpoint.TokenEndpoint*/, "roClient", "vBoxSecret");
            tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(profile.UserId, profile.Password, "openid").Result;
            
            if (tokenResponse.IsError)
            {
                throw new Exception("Getting token failed: " + tokenResponse.Error);
            }

            //TODO: getting userInfo
            var userInfoClient = new UserInfoClient(baseUrl + "/connect/userinfo"/*endpoint.UserInfoEndpoint*/);
            var userInfoResponse = userInfoClient.GetAsync(tokenResponse.AccessToken).Result;
            claims = userInfoResponse.Claims;

            //userId
            var idClaim = claims.Single(c => c.Type.Equals(JwtClaimTypes.Id));
            if (!idClaim.Value.Equals(profile.UserId))
            {
                throw new Exception("failed getting user info");
            }

        }


    }
}
