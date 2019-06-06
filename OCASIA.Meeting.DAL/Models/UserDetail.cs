using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
  public  class UserDetail   {
        public int UserDetailID { get; set; }
        public int TitleID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FirstLastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string AssitantEmail { get; set; }
        public string TelephoneNumber { get; set; }
        public int? CountryID { get; set; }
        public string Organization { get; set; }
        public string OrganizationDesignation { get; set; }
        public string OrganizationFunction { get; set; }
        public string Company { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public int? CountryOfResidanceID { get; set; }
        public string Nationality { get; set; }
        public DateTime? DOB { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string PassportCoptyPath { get; set; }
        public string GuestOf { get; set; }
        public string UploadedPicturePath { get; set; }
        public bool? IsGust { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }

        public bool? IsSubmitted { get; set; }
        public bool? IsCanclled { get; set; }
        public int? MeetingID { get; set; }

        public DateTime CreatedOn { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsPublish { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Users { get; set; }

        [ForeignKey("CountryID")]
        public virtual Mst_Country Countries { get; set; }

        public ICollection<TravelDetail> TravelDetails { get; set; }

        [ForeignKey("TitleID")]
        public virtual Mst_Title Titles { get; set; }

        [ForeignKey("MeetingID")]
        public virtual Meeting Meetings { get; set; }

    }
}
