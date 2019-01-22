using Trepub.Common;
using Trepub.Common.Entities;
using Trepub.Common.EntitiesExt;
using Trepub.Common.Exceptions;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Trepub.BSO
{
    public class RoleBSO
    {
        private readonly IRepository<RoleItem> roleItemRepo;
        private readonly IResourceRepository resourceRepo;
        private readonly IRepository<RoleItemResource> roleItemResourceRepo;
        private readonly IRepository<ProfileRole> profileRoleRepo;
        private static Dictionary<int, List<Resource>> roleResourcesCache = new Dictionary<int, List<Resource>>();

        public RoleBSO()
        {
            this.roleItemRepo = FacadeProvider.IfsFacade.GetRepository<RoleItem>();
            this.resourceRepo = FacadeProvider.IfsFacade.GetRepositoryByInterface<IResourceRepository>();
            this.roleItemResourceRepo = FacadeProvider.IfsFacade.GetRepository<RoleItemResource>();
            this.profileRoleRepo = FacadeProvider.IfsFacade.GetRepository<ProfileRole>();
        }

        public virtual void UpdateProfileRoles(Profile profile)
        {

            if (profile.ProfileRole != null && profile.ProfileRole.Count > 0)
            {
                var fetchedProfileRoles = profileRoleRepo.List(pr => pr.ProfileId == profile.ProfileId);

                //creating newly mentioned roles 
                foreach (var pr in profile.ProfileRole)
                {
                    if (fetchedProfileRoles.Where(i => i.RoleItemId == pr.RoleItemId).Count() <= 0)
                    {
                        pr.ProfileId = profile.ProfileId;
                        CreateProfileRole(profile.ProfileId, pr.RoleItemId);
                    }
                }

                //removing not mentioned roles 
                foreach(var pr in fetchedProfileRoles)
                {
                    if(profile.ProfileRole.Where(i => i.RoleItemId == pr.RoleItemId).Count() <= 0)
                    {
                        this.profileRoleRepo.Delete(pr);
                    }
                }
            }
        }

        public virtual ProfileRole CreateProfileRole(int profileId, int roleItemId)
        {
            var profileRole = new ProfileRole()
            {
                ProfileId = profileId,
                RoleItemId = roleItemId,
                CreationDate = DateTime.Now,
                Status = EntityConstants.ROLEPROFILE_STATUS_ACTIVE
            };
            this.profileRoleRepo.Add(profileRole);
            return profileRole;
        }

        public virtual bool HasPermission(int profileId, String resource)
        {
            var profileRoles = GetProfileRoles(profileId);
            foreach(var pr in profileRoles)
            {
                var roleItemResources = GetRoleItemResources(pr.RoleItemId);
                var matchedResources  = 
                    roleItemResources.Where(i => Regex.IsMatch(resource, i.ResourceContent, RegexOptions.IgnoreCase));
                if(matchedResources.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual List<ProfileRole> GetProfileRoles(int profileId)
        {
            return this.profileRoleRepo.List(i => i.ProfileId == profileId);
        }

        private List<Resource> GetRoleItemResources(int roleItemId)
        {
            if (roleResourcesCache.ContainsKey(roleItemId))
            {
                return roleResourcesCache[roleItemId];
            }
            //if its not fetched yet
            lock (roleResourcesCache)
            {
                var roleItemResources = this.roleItemResourceRepo.List(i => i.RoleItemId == roleItemId);
                List<int> roleItemResourceIds = roleItemResources.Select<RoleItemResource, int>(rir => rir.ResourceId).ToList();
                var roleResources = this.resourceRepo.List(i => roleItemResourceIds.Contains(i.ResourceId));
                roleResourcesCache.Add(roleItemId, roleResources);
                return roleResources;
            }
        }

        public virtual List<RoleItem> GetRoles()
        {
            return roleItemRepo.List(r => r.Status == EntityConstants.ROLEITEM_STATUS_ACTIVE);
        }

        public virtual RoleItem GetRole(int roleId)
        {
            return GetRole(new RoleItem() { RoleItemId = roleId });
        }

        public virtual RoleItem GetRole(RoleItem role)
        {
            RoleItem result = null;
            if (role.RoleItemId > 0)
            {
                result = roleItemRepo.GetSingle(r => r.RoleItemId == role.RoleItemId);
            }
            else
            {

                result = roleItemRepo.GetSingle(r => r.Name == role.Name &&
                                                    r.Status != EntityConstants.PROFILE_STATUS_DEACTIVE);
            }

            return result;
        }

        public virtual List<RoleItem> GetMyRoles(int profileId)
        {
            var profileRoles = GetProfileRoles(profileId);
            var result = new List<RoleItem>();
            foreach(ProfileRole pr in profileRoles)
            {
                result.Add(GetRole(pr.RoleItemId));
            }

            return result;
        }

        public virtual void DeleteProfileRole(int profileId, int roleId)
        {
            var profileRole = profileRoleRepo.GetSingle(pr => pr.ProfileId == profileId && pr.RoleItemId == roleId);
            profileRoleRepo.Delete(profileRole);
        }

        public virtual void CreateRoleItemResource(int resourceId, int roleItemId)
        {
            var roleItemRes = new RoleItemResource()
            {
                CreationDate = DateTime.Now,
                ResourceId = resourceId,
                RoleItemId = roleItemId,
                Status = EntityConstants.PROFILEROLE_STATUS_ACTIVE
            };
            roleItemResourceRepo.Add(roleItemRes);
        }

        public virtual List<ResourceExt> GetResources(ResourceExt resourceExt, int page = 0, int pageSize = 10)
        {
            return resourceRepo.GetResources(resourceExt, page, pageSize);
        }

        public virtual List<ResourceExt> GetMyResources(ResourceExt resourceExt)
        {
            return resourceRepo.GetResources(resourceExt, -1, -1);
        }

    }
}
