using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class PermissionLevel
    {
        public int PermissionLevelID { get; set; }
        public string Permission { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserModuleMapping> UserModuleMappings { get; set; }
    }
}
