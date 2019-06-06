namespace OCASIA.Meeting.DAL.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OCASIA.Meeting.DAL.OCASIAMeetingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        public string[] Roles = { "Admin", "User", "Participant" };

        protected override void Seed(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            //SeedRoles(context);
            //SeedMembers(context);
            //Module(context);
            //PermissionLevel(context);
            //UserModuleMapping(context);
            // SeedRegistrationTabs(context); 
           // SeedInvitationCategories(context);
        }

        #region Roles
        public void SeedRoles(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                foreach (string x in Roles)
                {
                    context.RoleCustoms.AddOrUpdate(f => f.RoleName,
                        new RoleCustom { RoleName = x });
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Seed RegistrationTabs
        public void SeedInvitationCategories(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                context.InviationCategorys.AddOrUpdate(e => e.InviationCategoryName,
                                 new Mst_InvitationCategory() { InviationCategoryName = "President", DisplayText = "President", Abbreviation = "NOCP", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },
                                                                 new Mst_InvitationCategory() { InviationCategoryName = "VicePresident", DisplayText = "Vice President", Abbreviation = "NOCVP", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { InviationCategoryName = "SecretaryGeneral", DisplayText= "Secretary General", Abbreviation = "NOCSG", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { InviationCategoryName = "EBMember",DisplayText= "EB Member", Abbreviation = "NOCEM", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { InviationCategoryName = "Member",DisplayText= "Member", Abbreviation = "NOCM", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { InviationCategoryName = "Guest",DisplayText= "Guest", Abbreviation = "NOCG", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "NOC", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { DisplayText = "President", InviationCategoryName = "President", Abbreviation = "OCAP", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "VicePresident", InviationCategoryName = "Vice President", Abbreviation = "OCAVP", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "Honorary Lifetime President", InviationCategoryName = "HonoraryLifetimePresident", Abbreviation = "OCAHLP", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "Chairman of Standing Committee", InviationCategoryName = "ChairmanofStandingCommittee", Abbreviation = "OCACSC", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "Director General", InviationCategoryName = "DirectorGeneral", Abbreviation = "OCADG", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "Member of Standing Committee", InviationCategoryName = "MemberofStandingCommittee", Abbreviation = "OCAMSC", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "EB Member", InviationCategoryName = "EBMember", Abbreviation = "OCAEBM", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "Staff", InviationCategoryName = "Staff", Abbreviation = "OCAS", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },
                                 new Mst_InvitationCategory() { DisplayText = "OCA Guest", InviationCategoryName = "OCAGuest", Abbreviation = "OCAG", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "OCA", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { DisplayText = "Guest", InviationCategoryName = "GUE", Abbreviation = "Guest", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "GUE", IsActive = true, IsPublish = true },

                                 new Mst_InvitationCategory() { DisplayText = "AGOC", InviationCategoryName = "AGOC", Abbreviation = "AGOC", CreatedBy = "cfff4e76-e90d-4de6-b556-02b43acab909", CreatedOn = DateTime.Now, GroupName = "AGOC", IsActive = true, IsPublish = true }

                               
                                 );
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Seed RegistrationTabs
        public void SeedRegistrationTabs(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                context.RegistrationTabs.AddOrUpdate(e => e.RegistrationTabName,
                                   new Mst_RegistrationTab { RegistrationTabName = "Information", DisplayText = "Information", IsActive = true },
                    new Mst_RegistrationTab { RegistrationTabName = "PersonalDetails", DisplayText = "Personal Details", IsActive = true },
                    new Mst_RegistrationTab { RegistrationTabName = "Travel", DisplayText = "Travel", IsActive = true },
                    new Mst_RegistrationTab { RegistrationTabName = "EventLocation", DisplayText = "Event Location", IsActive = true });
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Seed Members
        public void SeedMembers(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindByName("Admin");
                if (user == null)
                {
                    int RoleCustomID = context.RoleCustoms.SingleOrDefault(f => f.RoleName == "Admin").RoleCustomID;
                    string Email = "Admin@ocasia.com";
                    var admin = userManager.Create(new ApplicationUser { UserName = Email, Email = Email, RoleCustomID = RoleCustomID, IspasswordActive = true, IsActive = true }, "ocasia12345");
                    userManager.AddClaim(user.Id, new System.Security.Claims.Claim("FullName", Email));
                    context.Passwordhashs.AddOrUpdate(new Passwordhash { UserId = user.Id, Password = "ocasia12345" });
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Module
        public void Module(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                context.Modules.AddOrUpdate(e => e.ModuleName,
                                   new Module { Submenu = false, Group = 1, Mainmenu = true, DisplayModuleName = "CMS Users", ModuleName = "CMSUsers", IsActive = true },
                    new Module { Submenu = false, Group = 1, Mainmenu = false, DisplayModuleName = "Manage Roles", ModuleName = "ManageRoles", IsActive = true },
                    new Module { Submenu = false, Group = 1, Mainmenu = false, DisplayModuleName = "Manage Users", ModuleName = "ManageUsers", IsActive = true });


            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Permission Level
        public void PermissionLevel(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                context.PermissionLevels.AddOrUpdate(e => e.Permission,
                    new PermissionLevel { Permission = "All", IsActive = true },
                    new PermissionLevel { Permission = "Read", IsActive = true },
                    new PermissionLevel { Permission = "Write", IsActive = true },
                    new PermissionLevel { Permission = "Update", IsActive = true },
                    new PermissionLevel { Permission = "Delete", IsActive = true }
                    );
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region UserModule Mapping
        public void UserModuleMapping(OCASIA.Meeting.DAL.OCASIAMeetingContext context)
        {
            try
            {
                context.UserModuleMappings.AddOrUpdate(e => e.ModuleID,
                    new UserModuleMapping { ModuleID = context.Modules.SingleOrDefault(s => s.ModuleName == "CMSUsers").ModuleID, PermissionLevelID = context.PermissionLevels.SingleOrDefault(s => s.Permission == "All").PermissionLevelID, UserId = "cfff4e76-e90d-4de6-b556-02b43acab909", IsActive = true },
                    new UserModuleMapping { ModuleID = context.Modules.SingleOrDefault(s => s.ModuleName == "ManageRoles").ModuleID, PermissionLevelID = context.PermissionLevels.SingleOrDefault(s => s.Permission == "All").PermissionLevelID, UserId = "cfff4e76-e90d-4de6-b556-02b43acab909", IsActive = true },
                    new UserModuleMapping { ModuleID = context.Modules.SingleOrDefault(s => s.ModuleName == "ManageUsers").ModuleID, PermissionLevelID = context.PermissionLevels.SingleOrDefault(s => s.Permission == "All").PermissionLevelID, UserId = "cfff4e76-e90d-4de6-b556-02b43acab909", IsActive = true });
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}
