using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OCASIA.Meeting.DAL
{
    public partial class OCASIAMeetingContext : IdentityDbContext<ApplicationUser>
    {
        public OCASIAMeetingContext()
            : base("OCASIAMeetingContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }       
        public virtual DbSet<RoleCustom> RoleCustoms { get; set; }   
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<UserModuleMapping> UserModuleMappings { get; set; }
        public virtual DbSet<PermissionLevel> PermissionLevels { get; set; }
        public virtual DbSet<Passwordhash> Passwordhashs { get; set; }
        public virtual DbSet<RoleBasedPrivilege> RoleBasedPrivileges { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<Mst_RegistrationTab> RegistrationTabs { get; set; }
        public virtual DbSet<RegistrationTabDetail> RegistrationTabDetails { get; set; }
        public virtual DbSet<TravelDetail> TravelDetails { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
        public virtual DbSet<Mst_Title> Titles { get; set; }
        public virtual DbSet<Mst_Country> Countries { get; set; }
        public virtual DbSet<Mst_InvitationCategory> InviationCategorys { get; set; }
        public virtual DbSet<MeetingAccessKey> MeetingAccessKeys { get; set; }
        public virtual DbSet<Invitation> Invitations { get; set; }
        public virtual DbSet<Mst_NOC> NOCs { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public static OCASIAMeetingContext Create()
        {
            return new OCASIAMeetingContext();
        }
    }
}
