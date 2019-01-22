using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trepub.Common.Entities
{
    public partial class Profile
    {
        public Profile()
        {
            ProfileRole = new HashSet<ProfileRole>();
        }

        [Key]
        public int ProfileId { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
		[Key]
        [IgnoreUpdate]
        public DateTime LastModifiedTime { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? VerificationSendDate { get; set; }
        public int PartyId { get; set; }
        public ICollection<ProfileRole> ProfileRole { get; set; }
        public Party Party { get; set; }
    }
}
