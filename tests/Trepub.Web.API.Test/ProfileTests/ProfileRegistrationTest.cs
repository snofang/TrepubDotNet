using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels;
using System;
using System.Threading;
using Trepub.Web.API.ViewModels.Profile;

namespace Trepub.Web.API.Test.Clients.ProfileTests
{
    public class ProfileRegistrationTest : BaseWebTest
    {

        public override void RunTest()
        {
            Log("Scenario: normal profile.UserId should be a mobile number");
            NormalProfileUserIdShouldBeMobileNumber();

            Log("Scenario: Reset password without proper activation code should prevented");
            ResetPasswordWithoutProperActivationCodeShouldPrevented();

            Log("Scenario: Retrying to create the same profile between specified interval should be prevented");
            RetryCreatingProfileBetweenSpecifiedIntervalShouldBePrevented();
        }
        
        public void NormalProfileUserIdShouldBeMobileNumber()
        {
            this.client = new TestClient(new ProfileView()
            {
                UserId = "09sampleUser",
                Password = "samplePwd",
            });
            bool invalidUserErrorReceived = false;
            try
            {
                this.client.CreateProfile();
            }catch(AppException exc)
            {
                if (exc.ErrorCode.Equals(ErrorConstants.PROFILE_CREATTION_INVALIDUSERID))
                {
                    invalidUserErrorReceived = true;
                }
            }

            if (!invalidUserErrorReceived)
            {
                throw new Exception("NormalProfileUserIdShouldBeMobileNumber");
            }

        }

        public void ResetPasswordWithoutProperActivationCodeShouldPrevented()
        {
            this.client = new TestClient();
            var pv = this.client.CreateProfile();

            this.client.Profile.VerificationCode = string.Empty;
            bool errorReceived = false;
            try
            {
                this.client.ResetPassword();
            }
            catch (AppException exc)
            {
                if (exc.ErrorCode.Equals(ErrorConstants.PROFILE_ACTIVATION_INVALIDCODE))
                {
                    errorReceived = true;
                }
            }
            if (!errorReceived)
            {
                throw new Exception("password reset done without proper activation code.");
            }

            this.client.Profile.VerificationCode = pv.VerificationCode;
            this.client.ResetPassword();
            this.client.Login();
            this.client.UnregisterProfile();
        }

        public void RetryCreatingProfileBetweenSpecifiedIntervalShouldBePrevented()
        {
            //default profile registration
            this.client = new TestClient();
            var pv = this.client.CreateProfile();

            //it should be prevented to try creating profile before interval bassed.
            bool prevented = false;
            try
            {
                this.client.CreateProfile();
            }
            catch (AppException exc)
            {
                if(exc.ErrorCode.Equals(ErrorConstants.PROFILE_SENDVERIFICATIONCODE_INTERVAL_VIOLATED))
                {
                    prevented = true;
                }
            }
            if (!prevented)
            {
                throw new Exception("it should be prevented to retry creating profile without configured interval being passed");
            }

            //waiting for sendCode verification intervals to be passed for create profile retry"
            Thread.Sleep(10000);
            pv = this.client.CreateProfile();


            //unregistering profile
            this.client.Profile.VerificationCode = pv.VerificationCode;
            this.client.ResetPassword();
            this.client.Login();
            this.client.UnregisterProfile();
            this.client.Logout();

        }


    }
}
