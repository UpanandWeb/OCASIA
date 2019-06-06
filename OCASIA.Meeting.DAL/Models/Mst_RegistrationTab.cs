using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
   public class Mst_RegistrationTab
    {
        [Key]
        public int RegistrationTabID { get; set; }
        public string RegistrationTabName { get; set; }
        public string DisplayText { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public bool IsDynamic { get; set; } = true;
    }
}
