using OCASIA.Meeting.DAL.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OCASIA.Meeting.DAL.ApplicationModels
{
    public class MeetingViewModel
    {
        // Meeting Basic Details
        public int MeetingID { get; set; }
        public string Abbreviation { get; set; }
        [Required, Display(Name = "Meeting Name")]
        public string MeetingName { get; set; }
        [AllowHtml, Required, Display(Name = "Meeting Description")]
        public string MeetingDescription { get; set; }
        [Required, Display(Name = "Event StartDate")]
        public DateTime EventStartDate { get; set; }
        [Required, Display(Name = "Event EndDate")]
        public DateTime? EventEndDate { get; set; }
        [Required, Display(Name = "Application StartDate")]
        public DateTime? ApplicationStartDate { get; set; }
        [Required, Display(Name = "Application EndDate")]
        public DateTime? ApplicationEndDate { get; set; }
        public string PageBannerPath { get; set; }
        [AllowHtml, Required, Display(Name = "Faq Description")]
        public string FaqDescription { get; set; }
        public string FaqPath { get; set; }
        public string SchedulePath { get; set; }

        [Required(ErrorMessage = "Offical Email field is required."), StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string OfficalEmail { get; set; }
        [Required, Display(Name = "Address")]
        public string Address { get; set; }
        [Required, Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required, Display(Name = "Fax Number")]
        public string FaxNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublish { get; set; }
        public bool AllowRegistration { get; set; }

        // Information Tab Details        
        public int RegistrationTabID { get; set; }
        public string RegistrationTabs { get; set; }
        public List<string> RegistrationTabs123 { get; set; }
        [Required, Display(Name = "Basic Description")]
        public string BasicDescription { get; set; }
        [Required, Display(Name = "Description1")]
        public string Description1 { get; set; }
        public string Description1PicturePath { get; set; }
        public string FileName1 { get; set; }
        public string Description2 { get; set; }
        public string Description2PicturePath { get; set; }
        public string FileName2 { get; set; }
        public string Description3 { get; set; }
        public string Description3PicturePath { get; set; }
        public string FileName3 { get; set; }
        public string Description4 { get; set; }
        public string Description4PicturePath { get; set; }
        public string FileName4 { get; set; }
        public int DaysToGo { get { return (EventStartDate.Date - DateTime.Today.Date).Days; } }

        public string sDate
        {
            get
            {
                if (EventStartDate != null)
                {
                    return EventStartDate.Date.ToShortDateString();
                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return EventStartDate.ToShortDateString();
                    }
                    else
                        return EventStartDate.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }

        public string eDate
        {
            get
            {
                if (EventEndDate != null && EventEndDate.HasValue)
                {
                    return EventEndDate.Value.Date.ToShortDateString();

                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return EventEndDate.Value.ToShortDateString();
                    }
                    else
                        return EventEndDate.Value.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }
        public string rsDate
        {
            get
            {
                if (ApplicationStartDate != null && ApplicationStartDate.HasValue)
                {
                    return ApplicationStartDate.Value.Date.ToShortDateString();

                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return ApplicationStartDate.Value.ToShortDateString();
                    }
                    else
                        return ApplicationStartDate.Value.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }
        public string reDate
        {
            get
            {
                if (ApplicationEndDate != null && ApplicationEndDate.HasValue)
                {
                    return ApplicationEndDate.Value.Date.ToShortDateString();
                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return ApplicationEndDate.Value.ToShortDateString();
                    }
                    else
                        return ApplicationEndDate.Value.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }


    }
}
