using OCASIA.Meeting.Registration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OCASIA.Meeting.DAL.Operations;
using OCASIA.Meeting.DAL.ApplicationModels;
using System.Configuration;
using OCASIA.Meeting.DAL;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.IO;
using System.Text;

namespace OCASIA.Meeting.Registration.Controllers
{
    [Compress]
    [OCAExceptionHandler]
    [PreventDuplicateRequest]
    public class ParticipantController : Controller
    {
        static readonly string filepath = ConfigurationManager.AppSettings["FilePath"]?.ToString();
        static readonly string dateformat = ConfigurationManager.AppSettings["DateFormat"]?.ToString();

        RegistrationOperations dbOperations = new RegistrationOperations();

        static int MeetingID = 0;
        static MeetingDetail _meetingDetails;
        static public MeetingDetail MeetingDetails
        {
            get
            {
                if (MeetingID != 0 && (_meetingDetails == null || _meetingDetails.MeetingID != MeetingID)) { _meetingDetails = (new CommonOperations()).GetMeetingDetail(MeetingID); }

                return _meetingDetails;
            }
        }

        public string UserId { get { return User.Identity.GetUserId(); } }
        public int UserDetailsID { get { if (MeetingDetails != null) return MeetingDetails.UserDetailsID; return 0; } }
        public bool ReadOnly { get { if (MeetingDetails != null) return MeetingDetails.ReadOnly; return false; } }

        #region Email stuff
        readonly string smtpUsername = ConfigurationManager.AppSettings["smtp:Username"];
        readonly string smtpPassword = ConfigurationManager.AppSettings["smtp:Password"];
        readonly string Host = ConfigurationManager.AppSettings["smtp:Host"];
        readonly int Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp:Port"]);
        readonly bool EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtp:EnableSsl"]);
        readonly string CCEmail = ConfigurationManager.AppSettings["CCEmail"];
        #endregion

        #region Extra Stuff
        OCASIAMeetingUOW db = new OCASIAMeetingUOW();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;



        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion

        #region Constructor
        public ParticipantController()
        {
            // GetPageLevelDetails();

            ViewBag.MeetingDetails = MeetingDetails;
            if (MeetingDetails != null)
                ViewBag.FaqPath = MeetingDetails.FaqPath;

            ViewBag.IsSubmitted = ReadOnly;

            CommonOperations.FilePath = Helper.FilePath = filepath;
            CommonOperations.UI_DateFormt = Helper.UI_DateFormt = dateformat;
        }


        public ParticipantController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("ValidateUrl");
        }

        #region Login Related


        [Route("login")]
        public ActionResult Login()
        {
            MeetingID = 0;
            _meetingDetails = null;
            ViewBag.MeetingDetails = null;

            DAL.LoginViewModel model = new DAL.LoginViewModel();

            if (User.Identity.IsAuthenticated == false)
            {
                // ViewBag.ReturnUrl = returnUrl;
                if (Request.Cookies["Login"] != null)
                {
                    model.Email = Request.Cookies["Login"].Values["Email"];
                    model.Password = Request.Cookies["Login"].Values["Password"];
                    model.RememberMe = true;
                }
            }


            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login12(LoginViewModel model, string returnUrl)
        {
            // HttpCookie cookie = new HttpCookie("Login");


            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                switch (result)
                {
                    case SignInStatus.Success:

                        if (dbOperations.ValidateUserWithMeetingDetails(model.Email) == false)
                            return View("InValid", "In Valid User");

                        //if (model.RememberMe)
                        //{
                        //    cookie.Values.Add("Email", model.Email);
                        //    cookie.Values.Add("Password", model.Password);
                        //    cookie.Expires = DateTime.Now.AddDays(15);
                        //    Response.Cookies.Add(cookie);
                        //}
                        //else
                        //{
                        //    Response.Cookies.Remove("Login");
                        //    cookie.Expires = DateTime.Now.AddDays(-15);
                        //    cookie.Value = null;
                        //    Response.SetCookie(cookie);
                        //}


                        _meetingDetails = (new CommonOperations()).GetMeetingDetail(0, model.Email);
                        MeetingID = _meetingDetails.MeetingID;
                        //bool checktype=(new CommonOperations()).GetUserType(model.Email);
                        //if (checktype)
                        _meetingDetails.SuperAdmin = true;
                        ViewBag.MeetingDetails = MeetingDetails;

                        ViewBag.IsSubmitted = ReadOnly;
                        SetInformationTabDetails(1);
                        return RedirectToAction("InformationView");
                    default:
                        ViewBag.InvalidUser = true;
                        return View("Login");

                }

            }
            return View("InValid", "In Valid User");
        }



        [Filters.AuthAttribute.SuperAdminAuth]
        [HttpGet]
        public ActionResult LogOff(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                var user = db.Repository<ApplicationUser>().GetAll().Where(a => a.Email == Email).FirstOrDefault();
                if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    db.Repository<ApplicationUser>().Update(user);
                    db.SaveChanges();
                }
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                _meetingDetails = null;
                MeetingID = 0;

            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Participant");
        }


        [Filters.AuthAttribute.SuperAdminAuth]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.DonotShowBanner = true;
            return View();
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        [Filters.AuthAttribute.SuperAdminAuth]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            if (hasPassword)
            {
                //if (ModelState.IsValid)
                //{
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                    var Passwordhashs = db.Repository<Passwordhash>().GetAll().Where(a => a.UserId == UserId).FirstOrDefault();
                        if (Passwordhashs != null)
                        {
                            Passwordhashs.Password = model.NewPassword;
                            db.Repository<Passwordhash>().Update(Passwordhashs);
                            db.SaveChanges();
                        }
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        TempData["success"] = "Your password has been changed successfully!";
                    return RedirectToAction("PasswordConfirmation");

                    }
                    else
                    {
                        AddErrors(result);
                        TempData["error"] = "Please Enter Valid Current Password !";
                    return RedirectToAction("ChangePassword");

                }
                //}

            }
            return RedirectToAction("Login", "Participant");
        }

        public ActionResult PasswordConfirmation()
        {
            ViewBag.DonotShowBanner = false;
            return View("_PasswordConfirmation");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Participant");
        }

        #endregion

        public ActionResult FQA()
        {
            return File(MeetingDetails?.FaqPath, "application/jpg");
        }

        [AllowAnonymous]
        [Route("meeting-information")]
        public ActionResult MeetingDetailsView()
        {
            if (MeetingID == 0)
                MeetingID = dbOperations.GetCurrentMeetingID();
            MeetingPage pageAccess = MeetingPage.DoesNotExists;

            switch (MeetingID)
            {
                case 0: pageAccess = MeetingPage.DoesNotExists; break;
                case -1: pageAccess = MeetingPage.Experired; break;
                default:
                    pageAccess = MeetingPage.Active;
                    ViewBag.MeetingDetails = MeetingDetails;
                    ViewBag.Information = (new CommonOperations()).GetMeetingDetailbyID(MeetingID);

                    SetInformationTabDetails(1); break;
            }
            switch (pageAccess)
            {
                case MeetingPage.Active: return View();
                case MeetingPage.Experired:
                case MeetingPage.NotPermitted:
                default: return View("NotPermitted");
            }

        }

        [Route("key-validation")]
        [AllowAnonymous]
        public ActionResult ValidateUrl(int meetingID = 0)
        {
            if (meetingID == 0)
                MeetingID = meetingID = dbOperations.GetCurrentMeetingID();
            MeetingID = meetingID;//to store meetingID

            MeetingPage pageAccess = dbOperations.ValidateMeetingUrl(meetingID);

            ViewBag.MeetingDetails = MeetingDetails;

            switch (pageAccess)
            {
                case MeetingPage.Active: return View("AccessKeyValidation");
                case MeetingPage.Experired:
                case MeetingPage.NotPermitted: return View("NotPermitted");
            }
            return View("InValid", "In Valid Access Key");

        }

        [ValidateAntiForgeryToken]
        [Route("meeting-registration")]
        [AllowAnonymous]
        public ActionResult AccessKeyValidation(string AccessKey)
        {
            if (dbOperations.AccessKeyValidation(AccessKey, MeetingID))
            {
                ViewBag.TitleList = (new CommonOperations()).GetTitleList();
                string tabs = dbOperations.Tabs;
                return View("registration");
            }
            else
            {
                ViewBag.DonotShowBanner = true;
                return View("InValid", "In Valid Access Key");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> UserRegistration(ParticipantRegistration model)
        {
            MeetingDetails.UserName = model.GivenName;
            model.MeetingID = MeetingDetails.MeetingID;
            MeetingDetails.UserDetailsID = dbOperations.RegisterUser(model);
            #region Send Email   

            string Emailtemplate = string.Empty;
            Emailtemplate = "~/EmailTemplates/UserSignupConfirmation.html";
            using (StreamReader sr = new StreamReader(HttpContext.Server.MapPath(Emailtemplate)))
                if (sr != null)
                {
                    string Login = "<a href='" + Url.Action("Login", "Participant", new { }, "http") + "'>Click</a>";
                    StringBuilder HtmlPage = new StringBuilder();
                    string HTML = sr.ReadToEnd();
                    HtmlPage.Append(HTML
                        .Replace("[UserName]", model.Email)
                        .Replace("[Email]", model.Email)
                        .Replace("[Password]", model.Password)
                        .Replace("[Link]", Login)
                        );
                    #region Email                                      
                    DAL.ApplicationModels.Extension.SendMail(smtpUsername, smtpPassword, model.Email, Host, Port, EnableSsl, "Olympic Council of Asia Signup Confirmation", HtmlPage.ToString(), "Olympic Council of Asia Signup Confirmation", CCEmail);
                    #endregion
                }
            #endregion
            if (MeetingDetails.UserDetailsID == 0)
                return View("Error");
            else if (MeetingDetails.UserDetailsID == -1)
                return View("AlreadyExists");


            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                switch (result)
                {
                    case SignInStatus.Success:

                        SetInformationTabDetails(1);

                        return RedirectToAction("InformationView");
                }
            }
            return View(model);
        }

        [Route("information")]
        [Filters.AuthAttribute.SuperAdminAuth]
        public ActionResult InformationView()
        {

            SetInformationTabDetails(1);

            MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 1) el.IsSelected = true; else el.IsSelected = false; });
            return View("Information");
        }

        private void SetInformationTabDetails(int tabID)
        {
            switch (tabID)
            {
                case 1:
                    ViewBag.InformationTab = dbOperations.GetMeetingInfoTabDetils(MeetingID, 1);
                    break;

            }
        }


        #region Personal Details
        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("personal-details")]
        public ActionResult PersonalDetailsView()
        {
            UserDetailsModel model = new UserDetailsModel();
            if (MeetingDetails?.UserDetailsID != 0)
            {
                model = dbOperations.GetUserPersonalDetails(MeetingDetails.UserDetailsID);
            }
            ViewBag.CountryList = (new CommonOperations()).GetCountryList();
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 2) el.IsSelected = true; else el.IsSelected = false; });
            if (MeetingDetails.ReadOnly)
                return View("PersonalDetailsViewTab", model);

            return View("PersonalDetails", model);
        }

        [HttpGet]
        public ActionResult AddGuest(int UserDetailID = 0)
        {
            UserDetailsModel model = new UserDetailsModel();
            ViewBag.CountryList = (new CommonOperations()).GetCountryList();
            ViewBag.TitleList = (new CommonOperations()).GetTitleList();
            if (UserDetailID != 0)
            {
                model = dbOperations.GetUserPersonalDetails(UserDetailsID);
        }
            return PartialView("_AddGuest", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.AuthAttribute.SuperAdminAuth]
        public ActionResult AddPersonalDetails(UserDetailsModel model, string HttpPostedFilBase64, string FileName, string HttpPostedVFilBase64, string VFileName, string HttpPostePFilBase64, string PFileName, bool Isguest)
        {
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 2) el.IsSelected = true; else el.IsSelected = false; });
            if (MeetingDetails != null && MeetingDetails.ReadOnly && Isguest == false)
                return RedirectToAction("TravelDetailsView");
            if (!string.IsNullOrEmpty(HttpPostedFilBase64) && !string.IsNullOrEmpty(FileName))
            {
                string DefaultPath = "";
                string DefaulttargetPath = "";
                string folderpath = "";
                byte[] fileBytes = Convert.FromBase64String(HttpPostedFilBase64.Substring(HttpPostedFilBase64.IndexOf(',') + 1));
                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    // Image
                    string Originalfolderpath = "~/Participant/" + MeetingDetails.MeetingName;
                    folderpath = (Server.MapPath(Originalfolderpath));
                    DefaulttargetPath = (Server.MapPath(Originalfolderpath));
                    if (!(Directory.Exists(DefaulttargetPath)))
                    {
                        Directory.CreateDirectory(DefaulttargetPath);
                    }
                    string Originalpath = "~/Participant/" + MeetingDetails.MeetingName + "/" + FileName;
                    DefaultPath = (Server.MapPath(Originalpath));
                    DefaulttargetPath = (Server.MapPath(Originalpath));
                    using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                        ms.WriteTo(file);
                    model.UploadedPicturePath = Originalpath;
                }
            }
            if (!string.IsNullOrEmpty(HttpPostedVFilBase64) && !string.IsNullOrEmpty(VFileName))
            {
                string DefaultPath = "";
                string DefaulttargetPath = "";
                byte[] fileBytes = Convert.FromBase64String(HttpPostedVFilBase64.Substring(HttpPostedVFilBase64.IndexOf(',') + 1));
                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    // Image                     
                    string Originalpath = "~/Participant/" + MeetingDetails.MeetingName + "/" + VFileName;
                    DefaultPath = (Server.MapPath(Originalpath));
                    DefaulttargetPath = (Server.MapPath(Originalpath));
                    using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                        ms.WriteTo(file);
                    model.PassportCoptyPath = Originalpath;
                }
            }
            if (!string.IsNullOrEmpty(HttpPostePFilBase64) && !string.IsNullOrEmpty(PFileName))
            {
                string DefaultPath = "";
                string DefaulttargetPath = "";
                byte[] fileBytes = Convert.FromBase64String(HttpPostePFilBase64.Substring(HttpPostePFilBase64.IndexOf(',') + 1));
                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    // Pdf                     
                    string Originalpath = "~/Participant/" + MeetingDetails.MeetingName + "/" + PFileName;
                    DefaultPath = (Server.MapPath(Originalpath));
                    DefaulttargetPath = (Server.MapPath(Originalpath));
                    using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                        ms.WriteTo(file);
                    model.PassportCoptyPath = Originalpath;
                }
            }
            model.UserDetailID = MeetingDetails.UserDetailsID;
            //if (Isguest == false)
            //{
            //    model.UserDetailID = MeetingDetails.UserDetailsID;
            //}           
            model.UpdatedBy = User.Identity.GetUserId();
            model.CreatedBy = User.Identity.GetUserId();
            var state = dbOperations.UpdatePersonalDetails(model, Isguest);
            if (state && Isguest == false)
            {
                MeetingDetails.IsPersonalFilled = true;
                return RedirectToAction("TravelDetailsView");
            }
            else if (state && Isguest == true)
            {
                MeetingDetails.IsTravelFilled = true;
                return RedirectToAction("GuestsView");
            }
            else
                return View("Error");
        }
        #endregion

        #region Travel Details
        [Route("travel-details")]
        public ActionResult TravelDetailsView()
        {

            if (MeetingDetails?.UserDetailsID != 0)
            {
                MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 3) el.IsSelected = true; else el.IsSelected = false; });
                var data = dbOperations.GetTravelDetails(MeetingDetails.UserDetailsID);
                if (data != null && data.ReadOnly)
                    return View("TravelDetailsViewTab", data);
                else
                    return View("TravelDetails", data);
            }

            return View("TravelDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTravelDetails(TravelDetailsModel travelModel)
        {
            travelModel.CreatedBy = User.Identity.GetUserId();
            travelModel.UserDetailID = MeetingDetails.UserDetailsID;
            var state = dbOperations.UpdateTravelDetails(travelModel);
            if (state)
            {
                MeetingDetails.IsTravelFilled = true;
                return RedirectToAction("GuestsView");
            }
            else
                return View();
        }
        #endregion

        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("guest-details")]
        public ActionResult GuestsView()
        {
            ViewBag.ListOfGuests = dbOperations.GetListOfGuests(MeetingDetails.UserDetailsID);
            List<UserDetailsModel> guests = dbOperations.GetGuestDetails(MeetingDetails.UserDetailsID);

            MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 5) el.IsSelected = true; else el.IsSelected = false; });
            if (MeetingDetails.ReadOnly)
                return View("GuestDetailsViewTab", guests);
            return View("guest");
        }

        [Filters.AuthAttribute.SuperAdminAuth]
        public ActionResult AddGuest(List<UserDetail> guests = null)
        {
            if (guests == null || guests.Count == 0)
                return RedirectToAction("confirmation");

            var state = dbOperations.AddGuests();
            if (state)
            {
                return RedirectToAction("Confirmation");
            }
            else
                return View();
        }

        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("confirmation")]
        public ActionResult Confirmation()
        {
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.Name == "Confirmation") el.IsSelected = true; else el.IsSelected = false; });

            if (MeetingDetails.ReadOnly == false && MeetingDetails.IsPersonalFilled && MeetingDetails.IsTravelFilled)
                ViewBag.IsAllFilledNotSubmitted = true;
            else
                ViewBag.IsAllFilledNotSubmitted = false;

            return View();
        }
       

        // [Route("event-details")]
        [Filters.AuthAttribute.SuperAdminAuth]
        public ActionResult EventView()
        {
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.ID == 4) el.IsSelected = true; else el.IsSelected = false; });

            return View("Information");
        }

        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("preview")]
        public ActionResult Preview()
        {
            ViewBag.IsPreviewScreen = true;
            var state = dbOperations.GetPreviewDetails(UserDetailsID);
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.Name == "Preview") el.IsSelected = true; else el.IsSelected = false; });
            if (state != null)
            {
                MeetingDetails.IsPersonalFilled = state.IsPersonalFilled;
                MeetingDetails.IsGuestsFilled = state.IsGuestsFilled;
                MeetingDetails.IsTravelFilled = state.IsTravelFilled;
            }
            if (MeetingDetails?.ReadOnly == true)
                state.ShowSubmitButton = false;
            else if (MeetingDetails.IsPersonalFilled && MeetingDetails.IsTravelFilled)
                state.ShowSubmitButton = true;
            else
                state.ShowSubmitButton = false;


            return View(state);

            //var state = dbOperations.GetUserFullDetails(UserDetailsID);
            //if (state)
            //{
            //    return View("preview");
            //}
            //else
            //    return View();
        }
        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("submt-details")]
        public ActionResult FormSubmit()
        {
            var state = dbOperations.UpdateUserConfirmtion(UserDetailsID, true);
            if (state)
            {
                ViewBag.IsSubmitted = MeetingDetails.ReadOnly = true;

                SendConfirmationEmail();

                return View("preview");
            }
            else
                return View("Submit");
        }

        private void SendConfirmationEmail()
        {
            #region Send Email   

            string Emailtemplate = string.Empty;
            Emailtemplate = "~/EmailTemplates/Confirmation.html";
            using (StreamReader sr = new StreamReader(HttpContext.Server.MapPath(Emailtemplate)))
                if (sr != null)
                {
                    //  string Login = "<a href='" + Url.Action("Login", "Participant", new { }, "http") + "'>Click</a>";
                    StringBuilder HtmlPage = new StringBuilder();
                    string HTML = sr.ReadToEnd();
                    HtmlPage.Append(HTML);

                    //HtmlPage.Append(HTML
                    //    .Replace("[UserName]", model.Email)
                    //    .Replace("[Email]", model.Email)
                    //    .Replace("[Password]", model.Password)
                    //    .Replace("[Link]", Login)
                    //    );
                    #region Email                                      
                    DAL.ApplicationModels.Extension.SendMail(smtpUsername, smtpPassword, User.Identity.Name, Host, Port, EnableSsl, "Olympic Council of Asia Meeting Registration Confirmation", HtmlPage.ToString(), "", CCEmail);
                    #endregion
                }
            #endregion
        }

        [Filters.AuthAttribute.SuperAdminAuth]

        public ActionResult Submit(int userDetailsID, bool isSubmitted, bool IsCanclled)
        {
            var state = dbOperations.UpdateUserConfirmtion(userDetailsID, isSubmitted, IsCanclled);
            if (state)
            {
                return View("preview");
            }
            else
                return View();
        }

        [Filters.AuthAttribute.SuperAdminAuth]
        [Route("calcellation")]
        public ActionResult Cancellation()
        {
            MeetingDetails?.TabDetails?.ForEach(el => { if (el.Name == "Cancellation") el.IsSelected = true; else el.IsSelected = false; });

            return View();
        }
    }
}