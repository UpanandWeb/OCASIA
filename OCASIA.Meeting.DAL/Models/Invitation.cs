using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class Invitation
    {
        public int InvitationID { get; set; }
        public int MeetingID { get; set; }
        public int InvitationCategoryID { get; set; }
        public string InvitationAccessKeyName { get; set; }
        public int? NOCID { get; set; }

        public string InvitationUserName { get; set; }
        public string EmailID { get; set; }
        public string Description { get; set; }
        public string Subject{get;set;}

        public DateTime? SentDate { get; set; }
        public bool IsSent { get; set; }
        public bool IsDelivered { get; set; }
       
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
        [ForeignKey("InvitationCategoryID")]
        public virtual Mst_InvitationCategory InvitationCategories { get; set; }

        [ForeignKey("NOCID")]
        public virtual Mst_NOC NOC { get; set; }

    }
}
