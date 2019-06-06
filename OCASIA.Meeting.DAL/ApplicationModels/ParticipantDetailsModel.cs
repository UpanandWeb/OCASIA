using OCASIA.Meeting.DAL.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL.ApplicationModels
{
    public static class DateExtenstion
    {
        public static string DateFormat(this DateTime? str)
        {
            if (str != null && str.HasValue)
                return str.Value.ToString(CommonOperations.UI_DateFormt);
            return null;
        }
    }

    public class MeetingDetail
    {
        public int MeetingID { get; set; }
        public string MeetingName { get; set; }
        public string Tabs { get; set; }
        public string PageBanner { get; set; }
        public string FaqPath { get; set; }
        public string FaqDescription { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public DateTime StartDate { get; set; }
        public string TelephoneNumber { get; set; }
        public string UserName { get; set; }
        public int UserDetailsID { get; set; }
        public bool ReadOnly { get; set; } = false;
        public bool SuperAdmin { get; set; } = false;
        List<CollectionDetails> _tabDetails;
        public List<CollectionDetails> TabDetails
        {
            get
            {
                if (_tabDetails == null)
                {
                    _tabDetails = new List<CollectionDetails>();
                    foreach (var item in Tabs.Split(',').ToList())
                    {
                        string name = string.Empty;
                        CollectionDetails data = new CollectionDetails(); ;
                        switch (item)
                        {
                            case "1": data = new CollectionDetails() { ID = 1, Name = "Information", IsSelected = true }; break;
                            case "2": data = new CollectionDetails() { ID = 2, Name = "Personal Details", IsSelected = false }; break;
                            case "3": data = new CollectionDetails() { ID = 3, Name = "Travel", IsSelected = false }; break;
                            //case "4": data = new CollectionDetails() { ID = 4, Name = "Event Location", IsSelected = false }; break;
                            case "5": data = new CollectionDetails() { ID = 5, Name = "Guests", IsSelected = false }; break;
                        }
                        _tabDetails.Add(data);
                    }
                    _tabDetails.Add(new CollectionDetails() { Name = "Preview" });
                    _tabDetails.Add(new CollectionDetails() { Name = "Confirmation", IsToShow = !ReadOnly });
                    _tabDetails.Add(new CollectionDetails() { Name = "Cancellation", IsToShow = ReadOnly });
                }
                return _tabDetails;


            }
        }

        public int DaysToGo { get { return (StartDate.Date - DateTime.Today.Date).Days; } }

        public bool IsPersonalFilled { get; set; } = false;
        public bool IsTravelFilled { get; set; } = false;
        public bool IsGuestsFilled { get; set; } = false;
        public string SchedulePath { get; set; }
    }
    public class ParticipantRegistration
    {
        public int UserDetailID { get; set; }
        [Required]
        [Display(Name = "Title")]
        public int TitleID { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Given Name")]
        public string GivenName { get; set; }
        [Required]
        [Display(Name = "Family Name")]
        public string FamilyName { get; set; }
        public string FirstLastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Email field is required."), StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }
        public string ConformPasswod { get; set; }

        public int MeetingID { get; set; }
    }
    public class UserDetailsModel
    {
        // static string DateFormat = "dd MMM yyyy";
        public string MeetingName { get; set; }
        public int UserDetailID { get; set; }
        public int TitleID { get; set; }
        [Required]
        public string Title { get; set; }
        //public string Password { get; set; }
        //public string ConfirmPassword { get; set; }
        [Required, Display(Name = "Given Name")]
        public string GivenName { get; set; }
        [Required, Display(Name = "Family Name")]
        public string FamilyName { get; set; }
        public string FirstLastName { get; set; }
        [Required, Display(Name = "Gender")]
        public string Gender { get; set; }
        [Required, StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Assistant Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }
        [StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Assistant Mail"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string AssitantEmail { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string TelephoneNumber { get; set; }
        [Required]
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Required]
        [Display(Name = "Organization")]
        public string Organization { get; set; }
        public string OrganizationDesignation { get; set; }
        public string OrganizationFunction { get; set; }
        public string Company { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public int CountryOfResidanceID { get; set; }
        [Required]
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; }
        [Required]
        [Display(Name = "Passort Number")]
        public string PassportNumber { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; }
        [Required]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [Required]
        public string PassportCoptyPath { get; set; }
        public string GuestOf { get; set; }
        [Required]
        public string UploadedPicturePath { get; set; }
        public bool IsGust { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }
        public string CountryName { get; set; }
        public string CountryOfResidance { get; set; }
        public string EDate
        {
            get
            {
                if (ExpiryDate != null && ExpiryDate.HasValue)
                {
                    return ExpiryDate.Value.Date.ToShortDateString();

                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return ExpiryDate.Value.Date.ToShortDateString();
                    }
                    else
                        return ExpiryDate.DateFormat();
                }
                return "";
            }
        }
        public string IDate
        {
            get
            {
                if (IssueDate != null && IssueDate.HasValue)
                {
                    return IssueDate.Value.Date.ToShortDateString();
                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return IssueDate.Value.Date.ToShortDateString();
                    }
                    else
                        return IssueDate.Value.Date.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }
        public string DDOB
        {
            get
            {
                if (DOB != null && DOB.HasValue)
                {
                    return DOB.Value.Date.ToShortDateString();
                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return DOB.Value.Date.ToShortDateString();
                    }
                    else
                        return DOB.Value.Date.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }


        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsPublish { get; set; }
        public bool ReadOnly { get; set; }
    }

    public class TravelDetailsModel
    {
        public int TravelDetailID { get; set; }
        public int UserDetailID { get; set; }
        public string ArrivalAirport { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalFlightNumber { get; set; }
        public string DeparturelAirport { get; set; }
        [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public int NoCheckInBages { get; set; }
        public string Comments { get; set; }
        public string Hotel { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public bool ReadOnly { get; set; } = false;
        public string ADate
        {
            get
            {
                if (ArrivalDate != null && ArrivalDate.HasValue)
                {
                    return ArrivalDate.Value.Date.ToShortDateString();
                    if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    {
                        return ArrivalDate.Value.Date.ToShortDateString();
                    }
                    else
                        return ArrivalDate.Value.Date.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }
        public string DDate
        {
            get
            {
                if (DepartureDate != null && DepartureDate.HasValue)
                {
                    return DepartureDate.Value.Date.ToShortDateString();

                    //if (string.IsNullOrEmpty(CommonOperations.UI_DateFormt))
                    //{
                    //     return DepartureDate.Value.Date.ToShortDateString();
                    //}
                    //else
                    //    return DepartureDate.Value.Date.ToString(CommonOperations.UI_DateFormt);
                }
                return "";
            }
        }

        public DateTime CreatedOn { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsPublish { get; set; }

    }

    public class TabDetailModel
    {
        public int RegistrationTabDetailID { get; set; }

        public int MeetingID { get; set; }


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

        public string CreatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublish { get; set; }


    }

    public class PreviewDetails
    {

        public UserDetailsModel Details { get; set; }

        public TravelDetailsModel Travel { get; set; }

        public List<UserDetailsModel> Guests { get; set; }
        public bool ShowSubmitButton { get; set; }
        public bool IsPersonalFilled { get; set; } = false;
        public bool IsTravelFilled { get; set; } = false;
        public bool IsGuestsFilled { get; set; }
    }
}
