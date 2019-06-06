using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class RoleCustom
    {
        public int RoleCustomID { get; set; }
        [Required(ErrorMessage = "RoleName field is required."), DataType(DataType.Text), StringLength(50), Display(Name = "User name"), RegularExpression(@"^[0-9a-zA-Z\- \/_?:.,\s]+$", ErrorMessage = "Special characters are not allowed.")]
        public string RoleName { get; set; }       
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public virtual ICollection<RoleBasedPrivilege> RoleBasedPrivileges { get; set; }  

        public enum PermissionLevels
        {
            Read,
            Write,
            Update,
            Delete
        }
    }

  
}
