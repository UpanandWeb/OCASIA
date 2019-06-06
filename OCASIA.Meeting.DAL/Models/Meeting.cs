using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
  public  class Meeting
    {
     public int MeetingID { get; set; }
        public string Abbreviation { get; set; }
        public string MeetingName { get; set; }
        public string MeetingDescription { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public string PageBannerPath { get; set; }       
        public string FaqDescription { get; set; }
        public string FaqPath { get; set; }
        public string OfficalEmail { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public bool AllowRegistration { get; set; }
        public string RegistrationTabs { get; set; }
        public string SchedulePath { get; set; }

        public DateTime CreatedOn { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsPublish { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Users { get; set; }

        public ICollection<RegistrationTabDetail> RegistrationTabDetails { get; set; }

    }
}
