using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ApplicationUser()
        {
            this.CreatedDate = DateTime.Now;
        }
        public bool? IsInRoleUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IspasswordActive { get; set; }
        public int RoleCustomID { get; set; }
        public int? MemberTypeID { get; set; }
        public virtual RoleCustom RoleCustoms { get; set; }       
        public virtual ICollection<UserModuleMapping> UserModuleMappings { get; set; }
        public virtual ICollection<Passwordhash> Passwordhashs { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }
        public virtual ICollection<RegistrationTabDetail> RegistrationTabDetails { get; set; }
        public virtual ICollection<TravelDetail> TravelDetails { get; set; }
        

    }
}