using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Trepub.Common.Entities;
using Trepub.Common.Exceptions;
using Trepub.Common.Extensions;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using Trepub.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static Trepub.Common.Utils.MobileNumberUtils;
using Trepub.Common;
using Trepub.Common.DataStructures;
using Trepub.Common.EntitiesExt;

namespace Trepub.BSO
{
    public class ProfileBSO
    {
        private readonly IProfileRepository profileRepo;
        private readonly ILogger<ProfileBSO> logger;
        private readonly IDMapBSO idMapBSO;
        private readonly RoleBSO partyRoleBSO;
        private readonly IConfiguration configuration;
        private static Random random;
        private readonly IExternalServices services;


        public ProfileBSO()
        {
            this.profileRepo = FacadeProvider.IfsFacade.GetRepositoryByInterface<IProfileRepository>();
            this.logger = FacadeProvider.IfsFacade.GetLogger<ProfileBSO>();
            this.idMapBSO = BusinessFacade.GetBSO<IDMapBSO>();
            this.partyRoleBSO = BusinessFacade.GetBSO<RoleBSO>();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
            this.services = FacadeProvider.IfsFacade.GetExternalServices();
        }

        public virtual Profile GetProfile(Profile profile)
        {
            profile = GetProfileSlim(profile);

            if(profile != null)
            {
                profile.Party = BusinessFacade.GetBSO<PartyBSO>().GetParty(profile.PartyId);
            }
            return profile;
        }

        public virtual Profile GetProfileSlim(Profile profile)
        {
            Profile result = null;
            if (profile.ProfileId > 0)
            {
                result = GetProfileSlim(profile.ProfileId);
            }
            else
            {
                result = GetProfileSlim(profile.UserId);
            }
            return result;
        }

        public virtual void DeactivateProfile(int profileId)
        {
            var profile = profileRepo.GetSingle(p => p.ProfileId == profileId);
            profile.Status = EntityConstants.PROFILE_STATUS_DEACTIVE;
            UpdateProfileSlim(profile);
        }

        public virtual List<ProfileExt> GetProfiles(ProfileFilter filter, int? page, int? pageSize)
        {
            return profileRepo.GetProfiles(filter, page, pageSize);
        }

        public virtual int GetProfilesTotalCount(ProfileFilter profile)
        {
            return profileRepo.GetProfilesTotalCount(profile);
        }

        public virtual Profile UpdateProfile(Profile profile)
        {
            //updating profile itself not permited; just fetching existing one and filling missing data
            var p = GetProfileSlim(profile);
            profile.Party.PartyId = p.PartyId;

            //updating party
            BusinessFacade.GetBSO<PartyBSO>().UpdateParty(profile.Party);

            return GetProfile(profile);
        }

        public virtual void ChangePassword(int profileId, String oldPassword, string newPassword)
        {
            ValidatePassword(newPassword);
            var profile = profileRepo.GetSingle(p => p.ProfileId == profileId);
            if (!profile.Password.Equals(oldPassword.HashPassword()))
            {
                throw new AppException(ErrorConstants.PROFILE_PASSWORD_INVALID, "old password is not equal to current password");
            }
            profile.Password = newPassword.HashPassword();
            UpdateProfileSlim(profile);
        }

        public virtual void ResetPassword(Profile profile)
        {
            ValidatePassword(profile.Password);
            profile.Password = profile.Password.HashPassword();

            var fetchedProfile = GetProfileSlim(profile);
            if (fetchedProfile.VerificationCode.Equals(profile.VerificationCode))
            {
                fetchedProfile.Password = profile.Password;
            }
            else
            {
                throw new AppException(ErrorConstants.PROFILE_ACTIVATION_INVALIDCODE, "invalid verification code.", profile);
            }
            UpdateProfileSlim(fetchedProfile);
        }

        public virtual void SendVerificationCode(Profile profile, Boolean resend = false)
        {
            profile = GetProfileSlim(profile);

            //validation
            if (profile.VerificationSendDate.HasValue)
            {
                int interval = 300;
                int.TryParse(configuration.GetSection("LocalSettings")["VerificationCodeSendIntervalSeconds"], out interval);
                var forbitUntil = profile.VerificationSendDate.Value.AddSeconds(interval);
                if (forbitUntil.Ticks > DateTime.Now.Ticks)
                {
                    throw new AppException(ErrorConstants.PROFILE_SENDVERIFICATIONCODE_INTERVAL_VIOLATED,
                        "its not possible to resend verification code unti " + forbitUntil.ToString(), profile);
                }
            }

            String verificationCode = GetRandomNumber();
            //TODO: to send verification code via email or sms

            profile.VerificationSendDate = DateTime.Now;
            profile.VerificationCode = verificationCode;
            logger.LogInformation("verification code generated\n" + ObjectDumper.Dump(profile));
            UpdateProfileSlim(profile);
        }

        public virtual Profile CreateNormalProfile(Profile profile)
        {
            if (MobileNumberUtils.GetMsisdnFormat(profile.UserId) != MsisdnFormats.ZeroLeading)
            {
                throw new AppException(ErrorConstants.PROFILE_CREATTION_INVALIDUSERID, "Only zero-leading mobile number for userId is supported.");
            }

            lock (this)
            {
                var p = GetProfileSlim(profile.UserId);
                if (p == null)
                {
                    profile.ProfileRole = new List<ProfileRole>() { new ProfileRole() { RoleItemId = EntityConstants.ROLE_IDS_NORMALUSER } };
                    profile.Type = EntityConstants.PROFILE_TYPE_MOBILENUMBER;
                    profile.MobileNumber = profile.UserId;
                    profile.Password = GetRandomNumber();
                    profile = CreateProfile(profile);
                }
                else
                {
                    profile = GetProfileSlim(p);
                }
                SendVerificationCode(profile);
                return GetProfile(profile);
            }
        }

        public virtual Profile CreateProfile(Profile profile)
        {

            //role validation
            if (profile.ProfileRole.Where<ProfileRole>(r => r.RoleItemId == EntityConstants.ROLE_IDS_NORMALUSER).Count() > 0)
            {
                profile.Status = EntityConstants.PROFILE_STATUS_ACTIVE;

            }
            else
            {
                throw new Exception("unsupported profile role");
            }


            lock (this)
            {
                CheckProfileExistance(profile);

                //default party creation
                var party = BusinessFacade.GetBSO<PartyBSO>().CreatePersonParty();

                //profile
                profile.PartyId = party.PartyId;
                var profileRoles = profile.ProfileRole;
                profile = CreateProfileSlim(profile);

                //profileRoles
                profile.ProfileRole = profileRoles;
                this.partyRoleBSO.UpdateProfileRoles(profile);

                return profile;
            }

        }

        private Profile CreateProfileSlim(Profile profile)
        {
            ValidatePassword(profile.Password);
            profile.Password = profile.Password.HashPassword();
            profile.ProfileId = idMapBSO.GetNextID(EntityConstants.IDMAP_PROFILEID);
            profile.Status = EntityConstants.PROFILE_STATUS_ACTIVE;
            profile = profileRepo.Add(profile);
            logger.LogInformation("PartyBSO: profile created.\n" + ObjectDumper.Dump(profile));
            return profile;
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) ||
                password.Length < 6)
            {
                throw new AppException(ErrorConstants.PROFILE_PASSWORD_INVALIDSTRENGTH, "password length should be at least 6 character");
            }

        }

        private void CheckProfileExistance(Profile profile)
        {
            if (GetProfileSlim(profile.UserId) != null)
            {
                throw new AppException(ErrorConstants.PROFILE_CREATTION_DUPLICATE, "specfied profile already exists.", profile);
            }
        }

        public string GetRandomNumber()
        {
            if (random == null)
            {
                random = new Random(DateTime.Now.Minute);
            }
            return random.Next(0, 1000000).ToString("D6");
        }

        private Profile GetProfileSlim(int profileId)
        {
            return profileRepo.GetSingle(p => p.ProfileId == profileId);
        }

        private Profile GetProfileSlim(string userId)
        {
            return profileRepo.GetSingle(p => p.UserId == userId &&
                                                p.Status != EntityConstants.PROFILE_STATUS_DEACTIVE);
        }

        private void UpdateProfileSlim(Profile profile)
        {
            profileRepo.Update(profile);
            logger.LogInformation("profile updated\n" + ObjectDumper.Dump(profile));
        }


    }
}
