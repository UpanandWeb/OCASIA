using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class RoleBasedPrivilege
    {
        public int RoleBasedPrivilegeID { get; set; }
        public int ModuleID { get; set; }
        public int RoleCustomID { get; set; }
        public bool IsActive { get; set; }

        public virtual Module Modules { get; set; }
        public virtual RoleCustom RoleCustoms { get; set; }
    }
}
