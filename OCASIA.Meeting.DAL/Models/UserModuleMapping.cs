using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class UserModuleMapping
    {
        public int UserModuleMappingID { get; set; }
        public int ModuleID { get; set; }
        [Required]
        public virtual string UserId { get; set; }
        public int PermissionLevelID { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }
        public virtual Module Modules { get; set; }

        public virtual PermissionLevel PermissionLevels { get; set; }
        

    }
}
