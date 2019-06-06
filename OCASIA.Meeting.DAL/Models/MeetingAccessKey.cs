using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
   public class MeetingAccessKey
    {
        public int MeetingAccessKeyID { get; set; }
        public string MeetingAccessKeyName { get; set; }
        public int InviationCategoryID { get; set; }
        public int MeetingID { get; set; }
        public string EmailAddress { get; set; }
        public bool IsSent { get; set; }

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

       


        [ForeignKey("InviationCategoryID")]
        public virtual Mst_InvitationCategory InviationCategorys { get; set; }
    }
}
