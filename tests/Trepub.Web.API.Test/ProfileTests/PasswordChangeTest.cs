using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Web.API.Test.ProfileTests
{
    public class PasswordChangeTest : BaseWebTest
    {
        public override void RunTest()
        {
            client = new TestClient();
            client.RegisterProfile();
            client.Login();

            //doing some work that is permission required
            client.Profile_GetMine();

            //changing password via invalid old password
            var passwordChangeView = new ChangePasswordView()
            {
                NewPassword = "trepub12345",
                OldPassword = "invalidPassword"
            };
            bool invalidPasswordError = false;
            try
            {
                client.Profiles_changeMyPassword(passwordChangeView);
            }catch(AppException exc)
            {
                if (exc.ErrorCode.Equals(ErrorConstants.PROFILE_PASSWORD_INVALID))
                {
                    invalidPasswordError = true;
                }
            }
            if (!invalidPasswordError)
            {
                throw new Exception("providing invalid old password during password change should be prevented");
            }
            Log("change password having invalid old password is being prevented.");


            //changing password via corrent old password
            passwordChangeView = new ChangePasswordView()
            {
                NewPassword = "trepub12345",
                OldPassword = client.Profile.Password
            };
            client.Profiles_changeMyPassword(passwordChangeView);
            client.Profile.Password = passwordChangeView.NewPassword;

            //doing somthing with the old tocken 
            client.Profile_GetMine();
            Log("after change password the old tocken is still valid.");

            //doing somthing with new password 
            client.Logout();
            client.Login();
            //TODO
            //client.Order_mylist();
            Log("password changed is working properly");

            //unregistering profile
            client.UnregisterProfile();
            client.Logout();
        }
    }
}
