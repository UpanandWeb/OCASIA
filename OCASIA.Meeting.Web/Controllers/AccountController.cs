using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OCASIA.Meeting.DAL;
using System.Configuration;
using System.Data.Entity;
using System.Collections.Generic;
using OCASIA.Meeting.Web.Common;

namespace OCASIA.Meeting.Web.Controllers
{
    [OCAExceptionHandler]
    [PreventDuplicateRequest]
    public class AccountController : Controller
    {
        MemoryCacher memoryCacher = new MemoryCacher();

        #region Extra Stuff
        OCASIAMeetingUOW db = new OCASIAMeetingUOW();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            //Helper.AccessToken = Session["access_token"].ToString();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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
        #endregion
        
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            DAL.LoginViewModel model = new DAL.LoginViewModel();
            if (User.Identity.IsAuthenticated == false)
            {
                ViewBag.ReturnUrl = returnUrl;
                if (Request.Cookies["Login"] != null)
                {
                    model.Email = Request.Cookies["Login"].Values["Email"];
                    model.Password = Request.Cookies["Login"].Values["Password"];
                }
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
            return View(model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            HttpCookie cookie = new HttpCookie("Login");
            string message;
            int attemptsLeft = 0;
            int accessFailedCount = 0;
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                switch (result)
                {
                    // When token is verified correctly, clear the access failed count used for lockout
                    case SignInStatus.Success:
                        var res = await db.Repository<ApplicationUser>().GetAll().Where(a => a.Email == model.Email.Trim()).Include(a => a.RoleCustoms).FirstOrDefaultAsync();
                        if (res != null)
                        {
                            if (model.RememberMe)
                            {
                                cookie.Values.Add("Email", model.Email);
                                cookie.Values.Add("Password", model.Password);
                                cookie.Expires = DateTime.Now.AddDays(15);
                                Response.Cookies.Add(cookie);
                            }
                            else
                            {
                                Response.Cookies.Remove("Login");
                                cookie.Expires = DateTime.Now.AddDays(-15);
                                cookie.Value = null;
                                Response.SetCookie(cookie);
                            }
                            return RedirectToAction("Index", "Admin");
                        }
                        return RedirectToAction("Login", "Account");
                    // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                    // the user can not login until the lockout duration has passed
                    case SignInStatus.LockedOut:
                        message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                        TempData["error"] = message;
                        model.Errormessage = "Invalid";
                        return View(model);
                    case SignInStatus.Failure:
                    default:
                        attemptsLeft = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString()) - accessFailedCount;
                        message = string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);
                        TempData["error"] = message;
                        model.Errormessage = "Invalid";
                        return View(model);

                }
            }
            return View(model);
        }      
      
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            string errorMsg = string.IsNullOrEmpty(Email) ? "Login email field is required." : "";
            if (string.IsNullOrEmpty(errorMsg))
            {
                var user = UserManager.FindByEmail(Email);
                if (user != null)
                {
                    var token = UserManager.GeneratePasswordResetToken(user.Id);
                    if (!string.IsNullOrEmpty(token))
                    {
                        var appath = HttpContext.Request.Url.AbsoluteUri.Replace(HttpContext.Request.Url.AbsolutePath, "");
                        string action_link = appath + Url.Action("PasswordReset", "Account", new { uid = user.Id, rt = token });
                        string resetLink = "<a href='" + action_link + "'>Link</a>";
                        var fullname = user.Claims.First(f => f.ClaimType == "FullName").ClaimValue;
                        string path = Server.MapPath("~/EmailTemplates/password_reset.html");
                        string content = System.IO.File.ReadAllText(path);
                        string body = content.Replace("{{name}}", fullname).Replace("{{action_url}}", action_link).Replace("{{operating_system}}", Helper.GetUserPlatform()).Replace("{{browser_name}}", Request.Browser.Browser + " " + Request.Browser.Version).Replace("{{year}}", DateTime.Now.Year.ToString());

                        #region Email
                        string smtpUsername = ConfigurationManager.AppSettings["smtp:Username"];
                        string smtpPassword = ConfigurationManager.AppSettings["smtp:Password"];
                        string Host = ConfigurationManager.AppSettings["smtp:Host"];
                        int Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp:Port"]);
                        bool EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtp:EnableSsl"]);
                        DAL.ApplicationModels.Extension.SendMail(smtpUsername, smtpPassword, Email, Host, Port, EnableSsl, "Password reset link", body, "Password reset");                        
                        #endregion

                        TempData["success"] = "Successfully sent reset link on your email. Please check once ! Thank you.";
                    }
                    else
                    {
                        TempData["error"] = "Not registered user by this email.";
                    }
                }
                else
                {
                    TempData["error"] = "The email address <b> " + Email + " </b> is not registered user.Please try again.";
                }
            }
            else
            {
                TempData["error"] = errorMsg;
            }
            return PartialView("_Msg");
        }

        #region Check email with existing records
        public JsonResult IsAvailableUserAndEmail(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                Email = Email.Trim();
                if (Helper.IsValidEmail(Email.TrimEnd()))
                {
                    var email = db.Repository<ApplicationUser>().GetAll().Where(a => a.Email == Email).Select(a => a.Email).FirstOrDefault();
                    if (email != null)
                    {
                        return Json(new { Status = "1", ErrorMessage = "Email already exists, Please type different email" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Status = "0", ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }
        #endregion

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> PasswordReset()
        {
            string code = null;
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            }
            ViewBag.Code = code;
            ViewBag.UserId = user.Id;
            return code == null ? View("Error") : View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordReset(string uid, string rt, string Password, string ConfirmPassword)
        {
            string errorMsg = string.IsNullOrEmpty(uid) ? "Value for UserID field is required." : string.IsNullOrEmpty(Password) ? "Password field is required." : string.IsNullOrEmpty(ConfirmPassword) ? "Confirm Password is required." : (Password != ConfirmPassword) ? "Password and Confirm must be the same." : string.IsNullOrEmpty(rt) ? "Token required." : "";
            if (string.IsNullOrEmpty(errorMsg))
            {
                var exrec = await db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == uid).FirstOrDefaultAsync();
                if (exrec != null)
                {
                    var res = UserManager.ResetPassword(uid, rt, Password);
                    if (res.Succeeded)
                    {
                        var Passwordhashs = db.Repository<Passwordhash>().GetAll().Where(a => a.UserId == uid).FirstOrDefault();
                        if (Passwordhashs != null)
                        {
                            Passwordhashs.Password = Password;
                            db.Repository<Passwordhash>().Update(Passwordhashs);
                            db.SaveChanges();
                        }
                        TempData["success"] = "Your password has been reset successfully! ";
                    }
                    else
                    {
                        TempData["error"] = string.Join(",", res.Errors);
                    }
                }
                else
                {
                    TempData["error"] = "No user found by that email.";
                }
            }
            else
            {
                TempData["error"] = errorMsg;
            }
            PasswordReset();
            return View();
        }

        public string UserId { get { return User.Identity.GetUserId(); } }
        #region Menu Items Loading

        private IEnumerable<UserModuleMappingModel> LoadMenuItems()
        {

            IEnumerable<UserModuleMappingModel> lst;
            lst = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == UserId && a.IsActive).Include(a => a.Modules).Include(a => a.PermissionLevels)
            .Select(el => new UserModuleMappingModel() { Group = el.Modules.Group, PermissionLevelID = el.PermissionLevelID, ModuleID = el.ModuleID, ModuleName = el.Modules.ModuleName, MainMenu = el.Modules.Mainmenu, SubMenu = el.Modules.Submenu, DisplayModuleName = el.Modules.DisplayModuleName })
            .ToList();
            memoryCacher.Add("MenuItems" + UserId, lst, DateTimeOffset.UtcNow.AddMinutes(1));
            return lst;

        }

        IEnumerable<UserModuleMappingModel> menuItems;

        private void GetSideMenuList()
        {
            var result = memoryCacher.GetValue("MenuItems" + UserId);

            if (result == null)
                menuItems = LoadMenuItems();
            else
                menuItems = (IEnumerable<UserModuleMappingModel>)result;


            ViewBag.UserModuleMappinglist = menuItems;
            var LastLoginDate = db.Repository<ApplicationUser>().GetAll().Where(a => a.Id == UserId).Select(a => a.LastLoginDate).FirstOrDefault();
            ViewBag.LastLoginDate = LastLoginDate != null ? LastLoginDate : DateTime.Now;
        }

        public bool GetSideMenuList(string ModuleName)
        {
            if (menuItems == null)
                GetSideMenuList();
            return menuItems.FirstOrDefault(el => el.ModuleName == ModuleName) != null ? true : false;
        }

        #endregion

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            GetSideMenuList();
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

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var Passwordhashs = db.Repository<Passwordhash>().GetAll().Where(a => a.UserId == User.Identity.GetUserId()).FirstOrDefault();
                        if (Passwordhashs != null)
                        {
                            Passwordhashs.Password = model.NewPassword;
                            db.Repository<Passwordhash>().Update(Passwordhashs);
                            db.SaveChanges();
                        }
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        TempData["success"] = "Your password has been changed successfully!";
                    }
                    else
                    {
                        AddErrors(result);
                        TempData["error"] = "Please Enter Valid Current Password !";
                    }
                }

            }
            return RedirectToAction("Login", "Account");
        }
       
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
            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
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
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}