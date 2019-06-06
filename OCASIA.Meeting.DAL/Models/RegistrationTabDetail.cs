using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
   public class RegistrationTabDetail
    {
        public int RegistrationTabDetailID { get; set; }
        [Required]
        public int MeetingID { get; set; }
        [Required]
        
        public int RegistrationTabID { get; set; }
        public string BasicDescription { get; set; }
        public string Description1 { get; set; }
        public string Description1PicturePath { get; set; }
        public string Description2 { get; set; }
        public string Description2PicturePath { get; set; }
        public string Description3 { get; set; }
        public string Description3PicturePath { get; set; }
        public string Description4 { get; set; }
        public string Description4PicturePath { get; set; }

        public string FileName1 { get; set; }
        public string FileName1Path { get; set; }
        public string FileName2 { get; set; }
        public string FileName2Path { get; set; }
        public string FileName3 { get; set; }
        public string FileName3Path { get; set; }
        public string FileName4 { get; set; }
        public string FileName4Path { get; set; }

            public DateTime CreatedOn { get; set; }
            [Required]
            public string CreatedBy { get; set; }       
            public DateTime? UpdateOn { get; set; }
            public string UpdatedBy { get; set; }
            public bool IsActive { get; set; }
            public bool IsPublish { get; set; }

            [ForeignKey("CreatedBy")]
            public virtual ApplicationUser Users { get; set; }

            [ForeignKey("MeetingID")]
            public virtual Meeting Meetings { get; set; }

        [ForeignKey("RegistrationTabID")]
        public virtual Mst_RegistrationTab RegistrationTabs { get; set; }

    }
}
