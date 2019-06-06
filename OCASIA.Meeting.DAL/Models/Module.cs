using System.Collections.Generic;

namespace OCASIA.Meeting.DAL
{
    public class Module
    {
        public int ModuleID { get; set; }        
        public string ModuleName { get; set; }
        public string DisplayModuleName { get; set; }
        public int Group { get; set; }
        public bool Mainmenu { get; set; }
        public bool Submenu { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserModuleMapping> UserModuleMappings { get; set; }
        public virtual ICollection<RoleBasedPrivilege> RoleBasedPrivileges { get; set; }

    }
}
