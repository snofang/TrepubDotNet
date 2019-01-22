using Trepub.Web.API.Test.Common;
using Trepub.Web.API.ViewModels;
using Trepub.Web.API.ViewModels.Common;
using Trepub.Web.API.ViewModels.Profile;

namespace Trepub.Web.API.Test.TestClientBSO
{
    public static class PartyTestClientBSO 
    {

        public static ProfileView RegisterProfile(this TestClient client)
        {
            var pv = client.CreateProfile();
            client.Profile.VerificationCode = pv.VerificationCode;
            client.ResetPassword();
            return pv;
        }

        public static ProfileView CreateProfile(this TestClient client)
        {
            return client.Post<ProfileView>(client.Profile, "/api/profiles/createNormal");
        }

        public static void ResetPassword(this TestClient client)
        {
            client.Post<ProfileView>(client.Profile, "/api/profiles/resetPasswordMine");
        }

        public static void Profiles_changeMyPassword(this TestClient client, ChangePasswordView cpv)
        {
            client.Post<ChangePasswordView>(cpv, "/api/profiles/changePasswordMine");
        }

        public static void UnregisterProfile(this TestClient client)
        {
            client.Delete("/api/profiles/deleteMine");
        }

        public static ProfileView Profile_GetMine(this TestClient client)
        {
            return client.Get<ProfileView>("/api/profiles/getMine");
        }

        public static DataListView<ProfileListingView> Profile_List(this TestClient client, ProfileFilterView filter)
        {
            return client.Post<ProfileFilterView, DataListView<ProfileListingView>>(filter, "/api/profiles/list");
        }

        public static ProfileView Profile_UpdateMine(this TestClient client, ProfileView pv)
        {
            return client.Post<ProfileView>(pv, "/api/profiles/updateMine");
        }

    }
}
