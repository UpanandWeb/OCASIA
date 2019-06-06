using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Security.Application;
using OCASIA.Meeting.DAL;
using OCASIA.Meeting.DAL.ApplicationModels;
using OCASIA.Meeting.DAL.Operations;
using OCASIA.Meeting.Web.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OCASIA.Meeting.Web.Controllers
{
    [Filters.AuthAttribute.SuperAdminAuth]
    [Compress]
    //[OCAExceptionHandler]
    [PreventDuplicateRequest]
    public class AdminController : Controller
    {
        MemoryCacher memoryCacher = new MemoryCacher();

        #region Extra Stuff
        string _userRole = string.Empty;
        int _useRoleId;
        string smtpUsername = ConfigurationManager.AppSettings["smtp:Username"];
        string smtpPassword = ConfigurationManager.AppSettings["smtp:Password"];
        string Host = ConfigurationManager.AppSettings["smtp:Host"];
        int Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp:Port"]);
        bool EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtp:EnableSsl"]);
        public string UserId { get { return User.Identity.GetUserId(); } }
        public string UserRole
        {
            get
            {
                if (_userRole == string.Empty)
                    _userRole = Helper.CurrentUserRole();
                return _userRole;
            }
        }
        public int UserRoleID
        {
            get
            {
                if (_useRoleId == 0)
                    _useRoleId = Helper.CurrentUserRoleID();
                return _useRoleId;
            }
        }

        #region Constructor
        public AdminController()
        {
            //Helper.AccessToken = Session["access_token"].ToString();
        }
        #endregion        
        OCASIAMeetingUOW db = new OCASIAMeetingUOW();
        private OCASIAMeetingContext dbcontext = new OCASIAMeetingContext();
        #region Common Operation Instance
        CommonOperations _commonoperations;
        public CommonOperations Commonoperations
        {
            get
            {
                if (_commonoperations == null)
                    _commonoperations = new CommonOperations();
                return _commonoperations;
            }
        }
        #endregion

        #region Meeting Operation Instance
        MeetingOperations _MeetingOperations;
        public MeetingOperations MeetingOperations
        {
            get
            {
                if (_MeetingOperations == null)
                    _MeetingOperations = new MeetingOperations();
                return _MeetingOperations;
            }
        }
        #endregion

        #region Private Methods
        private string GetAbsoluteFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (fileName.Contains(":"))
                return Path.GetFileName(fileName);
            else return fileName;
        }
        #endregion

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        [HttpPost]
        public ActionResult GetSidemenulist(string title)
        {
            string UserId = User.Identity.GetUserId();
            var UserModuleMappinglist = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == UserId && a.IsActive).Include(a => a.Modules).Include(a => a.PermissionLevels)
                .Select(el => new UserModuleMappingModel() { Group = el.Modules.Group, PermissionLevelID = el.PermissionLevelID, ModuleID = el.ModuleID, ModuleName = el.Modules.ModuleName, MainMenu = el.Modules.Mainmenu })
                .ToList();
            ViewBag.UserModuleMappinglist = UserModuleMappinglist;

            ViewBag.title = title;
            var LastLoginDate = db.Repository<ApplicationUser>().GetAll().Where(a => a.Id == UserId).Select(a => a.LastLoginDate).FirstOrDefault();
            ViewBag.LastLoginDate = LastLoginDate;
            return PartialView("_SideMenu");
        }
        #endregion        

        #region Error Page
        //private ActionResult ErrorPage()
        //{
        //    return RedirectToAction("PageNotFound", "Home");
        //}

        private ActionResult UnAuthorized()
        {
            return RedirectToAction("UnAuthorized", "Home");
        }

        #endregion

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

        #region Venkat      

        public ActionResult Index()
        {
            //GetSideMenuList();
            return View();
        }

        #region Manage Roles CRUD Operation        
        [HttpGet]
        public ActionResult ManageRoles()
        {
            if (!GetSideMenuList("ManageRoles"))
                return UnAuthorized();

            string currentrole = Helper.CurrentUserRole();
            if (currentrole != null)
                ViewBag.Roles = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleName != currentrole).OrderByDescending(f => f.RoleCustomID).Distinct().ToList();
            else
                TempData["Error"] = "UserID not found!";
            return View();
        }

        #region Add Role

        [HttpGet]
        public ActionResult AddRole()
        {
            ViewBag.Modules = db.Repository<Module>().GetAll().Where(a => a.IsActive == true && a.Mainmenu == false && a.Submenu == false && a.ModuleID != 73 && a.ModuleID != 74).OrderByDescending(a => a.ModuleID).ToList();
            ViewBag.PermissionLevels = db.Repository<PermissionLevel>().GetAll().Where(a => a.IsActive == true).ToList();
            return PartialView("_AddRole");
        }

        [HttpPost]
        public ActionResult AddRolebasedprivileges(string[] CheckedmodulesAndPermissions, string RoleName)
        {
            if (CheckedmodulesAndPermissions != null && RoleName != null)
            {
                int RoleCustomID = 0;
                RoleCustom mdl = new RoleCustom();
                var res = db.Repository<RoleCustom>().GetAll().Where(r => r.RoleName == RoleName).FirstOrDefault();
                if (res == null)
                {
                    mdl.RoleName = RoleName;
                    db.Repository<RoleCustom>().Add(mdl);
                    db.SaveChanges();
                    RoleCustomID = mdl.RoleCustomID;
                }
                else
                {
                    if (res.RoleName != RoleName)
                    {
                        res.RoleName = RoleName;
                        db.Repository<RoleCustom>().Update(res);
                        db.SaveChanges();
                    }
                    RoleCustomID = res.RoleCustomID;
                }
                var RoleBasedPrivileges = db.Repository<RoleBasedPrivilege>().GetAll().Where(a => a.RoleCustomID == RoleCustomID).ToList();
                if (RoleBasedPrivileges.Count != 0)
                {
                    foreach (var item1 in RoleBasedPrivileges)
                    {
                        var res1 = db.Repository<RoleBasedPrivilege>().GetAll().Where(a => a.RoleCustomID == item1.RoleCustomID).FirstOrDefault();
                        if (res1 != null)
                        {
                            db.Repository<RoleBasedPrivilege>().Delete(res1);
                            db.SaveChanges();
                        }
                    }
                }
                RoleBasedPrivilege model = new RoleBasedPrivilege();
                foreach (string item in CheckedmodulesAndPermissions)
                {
                    string[] items = item.Split('_');
                    if (items != null)
                    {
                        int ModuleID = Convert.ToInt32(items[0]);
                        int Group = Convert.ToInt32(items[1]);
                        var res1 = db.Repository<RoleBasedPrivilege>().GetAll().Where(a => a.ModuleID == ModuleID && a.RoleCustomID == RoleCustomID).FirstOrDefault();
                        if (res1 == null)
                        {
                            model.ModuleID = ModuleID;
                            model.RoleCustomID = RoleCustomID;
                            model.IsActive = true;
                            db.Repository<RoleBasedPrivilege>().Add(model);
                            db.SaveChanges();
                        }
                    }
                }
            }
            TempData["Success"] = "Module Permissions Updated successfully !";
            ManageRoles();
            return PartialView("_BindRoles");

        }


        [HttpPost]
        public ActionResult AddRole(RoleCustom model)
        {

            {
                if (!string.IsNullOrEmpty(model.RoleName))
                {
                    try
                    {
                        var res = db.Repository<RoleCustom>().GetAll().Where(r => r.RoleName == model.RoleName).FirstOrDefault();
                        if (res == null)
                        {
                            RoleCustom NewRole = new RoleCustom { RoleName = model.RoleName };
                            db.Repository<RoleCustom>().Add(NewRole);
                            db.SaveChanges();
                            TempData["Success"] = "Role Added Successfully";
                        }
                        else
                        {
                            TempData["Error"] = "Role Already Exists !";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "Error occured";

                    }
                }
                ManageRoles();
                return PartialView("_BindRoles");
            }


        }

        #endregion

        #region Edit Role
        [HttpGet]
        public ActionResult EditRole(int RoleCustomID)
        {
            if (RoleCustomID != 0)
            {
                ViewBag.Modules = db.Repository<Module>().GetAll().Where(a => a.IsActive == true && a.Mainmenu == false && a.Submenu == false && a.ModuleID != 73 && a.ModuleID != 74).OrderByDescending(a => a.ModuleID).ToList();
                ViewBag.PermissionLevels = db.Repository<PermissionLevel>().GetAll().Where(a => a.IsActive == true).ToList();
                ViewBag.RoleBasedPrivilege = db.Repository<RoleBasedPrivilege>().GetAll().Where(a => a.RoleCustomID == RoleCustomID && a.IsActive == true).ToList();

                var model = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleCustomID == RoleCustomID).FirstOrDefault();
                return PartialView("_EditRole", model);
            }
            return PartialView("_EditRole");
        }

        [HttpPost]
        public ActionResult EditRole(RoleCustom model)
        {
            if (model != null)
            {
                try
                {
                    var res = db.Repository<RoleCustom>().GetAll().Where(r => r.RoleCustomID == model.RoleCustomID).FirstOrDefault();
                    if (res != null)
                    {
                        if (res.RoleName != model.RoleName)
                        {
                            var isexisted = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleName == model.RoleName).FirstOrDefault();
                            if (isexisted == null)
                            {
                                res.RoleName = model.RoleName;
                                db.Repository<RoleCustom>().Update(res);
                                db.SaveChanges();
                                TempData["Success"] = "Record Updated Successfully";
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Role ID" + model.RoleCustomID + " not found!";
                    }
                }
                catch
                {
                    TempData["Error"] = "Error occured";
                }
            }
            ManageRoles();
            return PartialView("_BindRoles");

        }
        #endregion

        #region DeleteRole
        [HttpGet]
        public ActionResult DeleteRole(int RoleCustomID)
        {
            var model = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleCustomID == RoleCustomID).Include(f => f.ApplicationUsers).FirstOrDefault();
            return PartialView("_DeleteRoleConfirm", model);
        }

        [HttpPost]
        public ActionResult DeleteRoleConfirm(int RoleCustomID = 0)
        {

            {
                var res = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleCustomID == RoleCustomID).FirstOrDefault();
                if (res != null)
                {
                    var RoleBasedPrivileges = db.Repository<RoleBasedPrivilege>().GetAll().Where(r => r.RoleCustomID == res.RoleCustomID).ToList();
                    if (RoleBasedPrivileges.Count != 0)
                    {
                        foreach (var x in RoleBasedPrivileges)
                        {
                            var RoleBasedPrivilege = db.Repository<RoleBasedPrivilege>().GetAll().Where(r => r.RoleBasedPrivilegeID == x.RoleBasedPrivilegeID).FirstOrDefault();
                            if (RoleBasedPrivilege != null)
                            {
                                db.Repository<RoleBasedPrivilege>().Delete(RoleBasedPrivilege);
                                db.SaveChanges();
                            }
                        }
                    }
                    var users = db.Repository<ApplicationUser>().GetAll().Where(r => r.RoleCustomID == res.RoleCustomID).ToList();
                    if (users.Count != 0)
                    {
                        foreach (var x in users)
                        {
                            var user = db.Repository<ApplicationUser>().GetAll().Where(r => r.RoleCustomID == x.RoleCustomID).FirstOrDefault();
                            if (user != null)
                            {
                                var UserModuleMappings = db.Repository<UserModuleMapping>().GetAll().Where(r => r.UserId == user.Id).ToList();
                                if (UserModuleMappings.Count != 0)
                                {
                                    foreach (var item in UserModuleMappings)
                                    {
                                        var UserModuleMapping = db.Repository<UserModuleMapping>().GetAll().Where(r => r.UserModuleMappingID == item.UserModuleMappingID).FirstOrDefault();
                                        if (UserModuleMapping != null)
                                        {
                                            db.Repository<UserModuleMapping>().Delete(UserModuleMapping);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                db.Repository<ApplicationUser>().Delete(user);
                                db.SaveChanges();
                            }
                        }
                    }
                    db.Repository<RoleCustom>().Delete(res);
                    db.SaveChanges();
                    TempData["Success"] = "Record Deleted Successfully";
                }
                else
                {
                    TempData["Success"] = "Role not found in your account!";
                }
            }

            {


            }
            ManageRoles();
            return PartialView("_BindRoles");
        }
        #endregion



        #endregion

        #region Manage Users CRUD Operation         

        [HttpGet]
        public ActionResult ManageUsers()
        {
            //if (!GetSideMenuList("ManageUsers"))
            //    return UnAuthorized();
            //GetSideMenuList();
            using (db = new OCASIAMeetingUOW())
            {
                ViewBag.Users = db.Repository<ApplicationUser>().GetAll().Include(f => f.RoleCustoms).Where(f => f.RoleCustoms.RoleName != UserRole && f.IsActive).OrderByDescending(a => a.Id).ToList();
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateUsersbyUserID(string UserID, bool IsActive)
        {
            if (!string.IsNullOrEmpty(UserID))
            {
                var model = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserID).FirstOrDefault();
                if (model != null)
                {
                    model.IsActive = IsActive;
                    db.Repository<ApplicationUser>().Update(model);
                    db.SaveChanges();
                }
                return Json(IsActive, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #region Add User
        [HttpGet]
        public ActionResult AddUser()
        {
            string UserID = Helper.CurrentUserID();
            string currentrole = Helper.CurrentUserRole();
            if (UserID != null)
            {
                ViewBag.Roles = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleName != currentrole && f.RoleName != "Participant").OrderByDescending(f => f.RoleCustomID).ToList();
            }
            return PartialView("_AddUser");
        }

        [HttpPost]
        public ActionResult AddUser(RegisterViewModel model)
        {
            if (model != null)
            {
                model.UserName = model.Email;
                if (!string.IsNullOrEmpty(model.UserName) && Helper.IsValidEmail(model.Email) && !string.IsNullOrEmpty(model.Password) && model.RoleCustomID != 0)
                {
                    var res = db.Repository<ApplicationUser>().GetAll().Where(f => f.Email == model.Email).FirstOrDefault();
                    if (res == null)
                    {
                        var user = new ApplicationUser()
                        {
                            UserName = model.Email,
                            Email = model.Email,
                            RoleCustomID = model.RoleCustomID,
                            IsActive = true,
                            IspasswordActive = true
                        };
                        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbcontext));
                        var ures = userManager.Create(user, model.Password);
                        if (ures.Succeeded)
                        {
                            userManager.AddClaim(user.Id, new System.Security.Claims.Claim("FullName", model.Email));
                            Passwordhash mdl = new Passwordhash()
                            {
                                UserId = user.Id,
                                Password = model.Password
                            };
                            db.Repository<Passwordhash>().Add(mdl);
                            db.SaveChanges();
                            #region Send Email   

                            string Emailtemplate = string.Empty;
                            Emailtemplate = "~/EmailTemplates/UserSignupConfirmation.html";
                            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("OCA.Web");
                            UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordResetToken"));
                            string Token = UserManager.GeneratePasswordResetToken(user.Id);
                            Token = HttpUtility.UrlEncode(Token);
                            if (!string.IsNullOrEmpty(Token))
                            {
                                using (StreamReader sr = new StreamReader(HttpContext.Server.MapPath(Emailtemplate)))
                                    if (sr != null)
                                    {
                                        string Login = "<a href='" + Url.Action("Login", "Account", new { }, "http") + "'>Click</a>";
                                        StringBuilder HtmlPage = new StringBuilder();
                                        string HTML = sr.ReadToEnd();
                                        HtmlPage.Append(HTML
                                            .Replace("[UserName]", user.UserName)
                                            .Replace("[Email]", user.Email)
                                            .Replace("[Password]", model.Password)
                                            .Replace("[Link]", Login)
                                            );
                                        #region Email                                      
                                        DAL.ApplicationModels.Extension.SendMail(smtpUsername, smtpPassword, user.Email, Host, Port, EnableSsl, "Olympic Council of Asia Signup Confirmation", HtmlPage.ToString(), "Olympic Council of Asia Signup Confirmation");

                                        #endregion
                                    }
                                TempData["success"] = "Please create your new password to continue!";
                            }
                            else
                            {
                                TempData["error"] = "Token generation failed. Please   after some time!";
                            }
                            #endregion
                            TempData["Success"] = "User (" + user.Email + ") Added Successfully!";
                        }
                        else
                        {
                            string err = "";
                            foreach (string x in ures.Errors.ToList())
                            {
                                err = err + "<br/>" + x;
                            }
                            TempData["Error"] = err;
                        }
                    }
                    else
                    {
                        TempData["Error"] = "User with Email Already Exists !";
                    }
                }
            }
            ManageUsers();
            return PartialView("_BindUsers");
        }

        #endregion

        #region Edit User
        [HttpGet]
        public ActionResult EditUser(string UserId)
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                string currentrole = Helper.CurrentUserRole();
                if (currentrole != null)
                {
                    ViewBag.Roles = db.Repository<RoleCustom>().GetAll().Where(f => f.RoleName != currentrole && f.RoleName != "Participant").OrderByDescending(f => f.RoleCustomID).ToList();
                }
                var res = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).Include(f => f.RoleCustoms).Include(a => a.Passwordhashs).FirstOrDefault();
                if (res != null)
                {
                    var model = new RegisterViewModel { UserId = res.Id, UserName = res.UserName, Email = res.Email, RoleCustomID = (int)res.RoleCustomID, Password = res.Passwordhashs.Count() != 0 ? res.Passwordhashs.FirstOrDefault().Password : "123456", ConfirmPassword = res.Passwordhashs.Count() != 0 ? res.Passwordhashs.FirstOrDefault().Password : "123456" };
                    return PartialView("_EditUser", model);
                }
                else
                {
                    TempData["error"] = "User record not found!";
                    return PartialView("_Msg");
                }
            }
            else
            {
                TempData["error"] = "User Id not found!";
            }
            return PartialView("_Msg");
        }

        [HttpPost]
        public ActionResult EditUser(RegisterViewModel model, string UserId = "")
        {
            model.UserName = model.Email;
            if (model != null && !string.IsNullOrEmpty(UserId) && Helper.IsValidEmail(model.Email))
            {
                model.UserName = model.Email;
                if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Email) && model.RoleCustomID != 0)
                {
                    var res = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).FirstOrDefault();
                    if (res != null)
                    {
                        res.UserName = model.Email;
                        res.Email = model.Email;
                        res.RoleCustomID = model.RoleCustomID;
                        res.IsActive = true;
                        var Passwordhashs = db.Repository<Passwordhash>().GetAll().Where(a => a.UserId == UserId).FirstOrDefault();
                        if (Passwordhashs != null)
                        {
                            db.Repository<ApplicationUser>().Update(res);
                            db.SaveChanges();
                            Passwordhashs.Password = model.Password;
                            db.Repository<Passwordhash>().Update(Passwordhashs);
                            db.SaveChanges();
                        }
                        string code = null;
                        ApplicationUser user = UserManager.FindByEmail(model.Email);
                        if (user != null)
                        {
                            code = UserManager.GeneratePasswordResetToken(user.Id);
                        }
                        var res1 = UserManager.ResetPassword(UserId, code, model.Password);
                        if (res1.Succeeded)
                        {
                            TempData["Success"] = "User (" + model.UserName + ") Updated Successfully";
                        }
                    }
                    else
                    {
                        TempData["error"] = "Record not found!";
                    }
                }
                else
                {
                    #region Else Block
                    if (string.IsNullOrEmpty(UserId))
                    {
                        TempData["error"] = "Reqired User Id!";
                    }
                    else if (string.IsNullOrEmpty(model.UserName))
                    {
                        TempData["error"] = "Reqired Username can't be empty. Please enter username and   again!";
                    }
                    else if (string.IsNullOrEmpty(model.Email))
                    {
                        TempData["error"] = "Reqired Email can't be empty. Please enter email and   again!";
                    }
                    else if (model.RoleCustomID == 0)
                    {
                        TempData["error"] = "Reqired Role can't be empty. Please select role and   again!";
                    }
                    #endregion
                }
            }
            ManageUsers();
            return PartialView("_BindUsers");
        }
        #endregion

        #region Delete User
        [HttpGet]
        public ActionResult DeleteUser(string UserId)
        {
            var model = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).FirstOrDefault();
            return PartialView("_DeleteUserConfirm", model);
        }


        [HttpPost]
        public ActionResult DeleteUserConfirm(string UserId)
        {
            if (UserId != null)
            {
                var thisuser = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).FirstOrDefault();
                if (thisuser != null)
                {
                    thisuser.IsActive = false;
                    db.Repository<ApplicationUser>().Update(thisuser);
                    db.SaveChanges();
                    TempData["Success"] = "User (" + thisuser.UserName + ") Deleted Successfully";
                }
                else
                {
                    TempData["error"] = "User (" + thisuser.UserName + ") record not found!";
                }
            }
            ManageUsers();
            return PartialView("_BindUsers");
        }

        [HttpGet]
        public ActionResult DeleteUserTrash(string UserId)
        {
            var model = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).FirstOrDefault();
            return PartialView("_DeleteUserTrashConfirm", model);
        }

        [HttpPost]
        public ActionResult DeleteTrashUserConfirm(string UserId)
        {

            {
                if (UserId != null)
                {
                    var thisuser = db.Repository<ApplicationUser>().GetAll().Where(f => f.Id == UserId).FirstOrDefault();
                    if (thisuser != null)
                    {
                        thisuser.IsActive = false;
                        db.Repository<ApplicationUser>().Update(thisuser);
                        db.SaveChanges();
                        TempData["Success"] = "User (" + thisuser.UserName + ") Trashed Successfully";
                    }
                    else
                    {
                        TempData["error"] = "User (" + thisuser.UserName + ") record not found!";
                    }
                }

                ManageUsers();
                return PartialView("_BindUsers");
            }


        }
        #endregion

        #region Check email with existing records
        public JsonResult IsAvailableUserAndEmail(string Username = null, string Email = null)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                Email = Email.Trim();
                if (Helper.IsValidEmail(Email))
                {
                    var email = db.Repository<ApplicationUser>().GetAll().Where(a => a.Email == Email).FirstOrDefault();
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

        #region Addprivileges
        [HttpGet]
        public ActionResult Addprivileges(string UserId, int RoleCustomID)
        {
            if (!string.IsNullOrEmpty(UserId) && RoleCustomID != 0)
            {

                ViewBag.Roles = db.Repository<RoleCustom>().GetAll().Where(a => a.RoleCustomID != (int)DAL.ApplicationModels.EnumsClass.RoleEnum.Admin || a.RoleCustomID != (int)DAL.ApplicationModels.EnumsClass.RoleEnum.User).ToList();
                ViewBag.Modules = db.Repository<RoleBasedPrivilege>().GetAll().Include(a => a.Modules).Where(a => a.RoleCustomID == RoleCustomID && a.IsActive == true && a.ModuleID != 73 && a.ModuleID != 74).OrderByDescending(a => a.ModuleID).ToList();
                ViewBag.PermissionLevels = db.Repository<PermissionLevel>().GetAll().Where(a => a.IsActive == true).ToList();
                ViewBag.UserModuleMapping = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == UserId && a.IsActive == true).ToList();
                List<SelectListItem> bindRecords = new List<SelectListItem>();
                bindRecords.Add(new SelectListItem { Text = "", Value = "" });
                ViewBag.Users = bindRecords.ToList();
                ViewBag.UserId = UserId;
                ViewBag.RoleCustomID = RoleCustomID;
            }
            return PartialView("_Addprivileges");
        }

        [HttpPost]
        public ActionResult Addprivileges(string[] CheckedmodulesAndPermissions, string UserId)
        {
            if (CheckedmodulesAndPermissions != null && UserId != null)
            {
                var UserModuleMappings = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == UserId).ToList();
                if (UserModuleMappings.Count != 0)
                {
                    foreach (var item1 in UserModuleMappings)
                    {
                        var res1 = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == item1.UserId).FirstOrDefault();
                        if (res1 != null)
                        {
                            db.Repository<UserModuleMapping>().Delete(res1);
                            db.SaveChanges();
                        }
                    }
                }
                UserModuleMapping model = new UserModuleMapping();
                List<int> groups = new List<int>();
                foreach (string item in CheckedmodulesAndPermissions)
                {
                    string[] items = item.Split('_');
                    if (items != null)
                    {
                        int ModuleID = Convert.ToInt32(items[0]);
                        int PermissionLevelID = Convert.ToInt32(items[1]);
                        int Group = Convert.ToInt32(items[2]);
                        var res = db.Repository<UserModuleMapping>().GetAll().Where(a => a.ModuleID == ModuleID && a.PermissionLevelID == PermissionLevelID && a.UserId == UserId).FirstOrDefault();
                        if (res == null)
                        {
                            if (!groups.Contains(Group))
                            {
                                groups.Add(Group);
                            }

                            model.ModuleID = ModuleID;
                            model.PermissionLevelID = PermissionLevelID;
                            model.UserId = UserId;
                            model.IsActive = true;
                            db.Repository<UserModuleMapping>().Add(model);
                            db.SaveChanges();
                        }
                    }
                }
                int groupId;
                foreach (var item in groups)
                {
                    groupId = item;

                    #region Main menu selection 
                    if (item == 9 || item == 10 || item == 11 || item == 12 || item == 13)
                        groupId = 8;

                    if (item == 31 || item == 32 || item == 33)
                        groupId = 3;


                    var MainmenuID = db.Repository<Module>().GetAll().Where(a => a.Group == groupId && a.Mainmenu == true).Select(a => a.ModuleID).FirstOrDefault();
                    var Mainmenu = db.Repository<UserModuleMapping>().GetAll().Where(a => a.ModuleID == MainmenuID && a.UserId == UserId).FirstOrDefault();
                    if (Mainmenu == null)
                    {
                        model.ModuleID = MainmenuID;
                        model.PermissionLevelID = 1;
                        model.UserId = UserId;
                        model.IsActive = true;
                        db.Repository<UserModuleMapping>().Add(model);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Sub menu selection 
                    var SubmenuID = db.Repository<Module>().GetAll().Where(a => a.Group == item && a.Submenu == true).Select(a => a.ModuleID).FirstOrDefault();
                    if (SubmenuID == 0)
                        continue;
                    var Submenu = db.Repository<UserModuleMapping>().GetAll().Where(a => a.ModuleID == SubmenuID && a.UserId == UserId).FirstOrDefault();
                    if (Submenu == null)
                    {
                        model.ModuleID = SubmenuID;
                        model.PermissionLevelID = 1;
                        model.UserId = UserId;
                        model.IsActive = true;
                        db.Repository<UserModuleMapping>().Add(model);
                        db.SaveChanges();
                    }
                    #endregion
                }
            }
            else
            {
                var UserModuleMappings = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == UserId).ToList();
                if (UserModuleMappings.Count != 0)
                {
                    foreach (var item1 in UserModuleMappings)
                    {
                        var res1 = db.Repository<UserModuleMapping>().GetAll().Where(a => a.UserId == item1.UserId).FirstOrDefault();
                        if (res1 != null)
                        {
                            db.Repository<UserModuleMapping>().Delete(res1);
                            db.SaveChanges();
                        }
                    }
                }
            }
            string Retval = "Module Permissions Updated successfully !";
            return Json(Retval, JsonRequestBehavior.AllowGet);

        }
        #endregion

        [HttpGet]
        public JsonResult GetUsersbyUserID(int RoleCustomID)
        {
            if (RoleCustomID != 0)
            {
                var res = (from a in db.Repository<ApplicationUser>().GetAll().Where(a => a.RoleCustomID == RoleCustomID)
                           select new SelectListItem
                           {
                               Text = a.UserName,
                               Value = a.Id.ToString()
                           }).ToList();

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public ActionResult Meeting()
        {
            if (_MeetingOperations == null)
                _MeetingOperations = new MeetingOperations();
            ViewBag.GetMeetingDetails = _MeetingOperations.GetMeetingDetails();
            return View();
        }

        [HttpGet]
        public ActionResult AddMeeting()
        {
            if (_commonoperations == null)
                _commonoperations = new CommonOperations();
            ViewBag.RegistrationTabList = Commonoperations.GetRegistrationTabList();
            return PartialView("_AddMeeting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddMeeting(MeetingViewModel model, string HttpPostedFilBase64, string FileName, string HttpPostedVFilBase64, string VFileName, string HttpPostedSFilBase64, string SFileName, string HttpPostedDesc1FilBase64, string Desc1FileName, string HttpPostedDesc2FilBase64, string Desc2FileName, string HttpPostedDesc3FilBase64, string Desc3FileName, string HttpPostedDesc4FilBase64, string Desc4FileName)
        {
            if (model != null)
            {
                string DefaultPath = "";
                string DefaulttargetPath = "";
                string folderpath = "";
                if (_commonoperations == null)
                    _commonoperations = new CommonOperations();
                // Basic Deatils Images

                #region Basic Deatils Upload Images
                #region Banner Image
                if (!string.IsNullOrEmpty(HttpPostedFilBase64) && !string.IsNullOrEmpty(FileName))
                {
                    string filename = GetAbsoluteFileName(FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedFilBase64.Substring(HttpPostedFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        // Image
                        string Originalfolderpath = "~/UploadFiles/" + model.MeetingName;
                        folderpath = (Server.MapPath(Originalfolderpath));
                        DefaulttargetPath = (Server.MapPath(Originalfolderpath));
                        if (!(Directory.Exists(DefaulttargetPath)))
                        {
                            Directory.CreateDirectory(DefaulttargetPath);
                        }
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.PageBannerPath = Originalpath;
                    }
                }
                #endregion

                #region FAG PDF
                if (!string.IsNullOrEmpty(HttpPostedVFilBase64) && !string.IsNullOrEmpty(VFileName))
                {
                    // PDF
                    string filename = GetAbsoluteFileName(VFileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedVFilBase64.Substring(HttpPostedVFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.FaqPath = Originalpath;
                    }
                }
                #endregion

                #region Schedule PDF
                if (!string.IsNullOrEmpty(HttpPostedSFilBase64) && !string.IsNullOrEmpty(SFileName))
                {
                    // PDF
                    string filename = GetAbsoluteFileName(SFileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedSFilBase64.Substring(HttpPostedSFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.SchedulePath = Originalpath;
                    }
                }
                #endregion
                #endregion

                // Registration Details Images
                #region Registration Details Upload Images

                #region Description1 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc1FilBase64) && !string.IsNullOrEmpty(Desc1FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc1FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc1FilBase64.Substring(HttpPostedDesc1FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description1PicturePath = Originalpath;
                        model.FileName1 = Desc1FileName;
                    }
                }
                #endregion

                #region Description2 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc2FilBase64) && !string.IsNullOrEmpty(Desc2FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc2FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc2FilBase64.Substring(HttpPostedDesc2FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description2PicturePath = Originalpath;
                        model.FileName2 = Desc2FileName;
                    }
                }
                #endregion

                #region Description3 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc3FilBase64) && !string.IsNullOrEmpty(Desc3FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc3FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc3FilBase64.Substring(HttpPostedDesc3FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description3PicturePath = Originalpath;
                        model.FileName3 = Desc3FileName;
                    }
                }
                #endregion

                #region Description4 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc4FilBase64) && !string.IsNullOrEmpty(Desc4FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc4FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc4FilBase64.Substring(HttpPostedDesc4FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description4PicturePath = Originalpath;
                        model.FileName4 = Desc4FileName;
                    }
                }
                #endregion 
                #endregion

                model.CreatedBy = User.Identity.GetUserId();
                bool obj = Commonoperations.AddMettingDeatils(model);
                if (obj == true)
                {
                    TempData["Success"] = "Meeting created successfully !!";
                }
                else
                {
                    TempData["Error"] = "Error occured while Meeting creation !!";
                }
            }
            Meeting();
            return PartialView("_BindMeetings");
        }


        [HttpGet]
        public ActionResult EditMeeting(int MeetingID)
        {
            if (MeetingID != 0)
            {
                if (_commonoperations == null)
                    _commonoperations = new CommonOperations();
                ViewBag.RegistrationTabList = Commonoperations.GetRegistrationTabList();                
                var model = Commonoperations.GetMeetingDetailbyID(MeetingID);                
                return PartialView("_EditMeeting", model);
            }
            return PartialView("_EditMeeting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditMeeting(MeetingViewModel model, string HttpPostedFilBase64, string FileName, string HttpPostedVFilBase64, string VFileName, string HttpPostedSFilBase64, string SFileName, string HttpPostedDesc1FilBase64, string Desc1FileName, string HttpPostedDesc2FilBase64, string Desc2FileName, string HttpPostedDesc3FilBase64, string Desc3FileName, string HttpPostedDesc4FilBase64, string Desc4FileName)
        {
            if (model != null)
            {
                string DefaultPath = "";
                string DefaulttargetPath = "";
                string folderpath = "";
                if (_commonoperations == null)
                    _commonoperations = new CommonOperations();
                // Basic Deatils Images

                #region Basic Deatils Upload Images
                #region Banner Image
                if (!string.IsNullOrEmpty(HttpPostedFilBase64) && !string.IsNullOrEmpty(FileName))
                {
                    string filename = GetAbsoluteFileName(FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedFilBase64.Substring(HttpPostedFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        // Image
                        string Originalfolderpath = "~/UploadFiles/" + model.MeetingName;
                        folderpath = (Server.MapPath(Originalfolderpath));
                        DefaulttargetPath = (Server.MapPath(Originalfolderpath));
                        if (!(Directory.Exists(DefaulttargetPath)))
                        {
                            Directory.CreateDirectory(DefaulttargetPath);
                        }
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.PageBannerPath = Originalpath;
                    }
                }
                #endregion

                #region FAG PDF
                if (!string.IsNullOrEmpty(HttpPostedVFilBase64) && !string.IsNullOrEmpty(VFileName))
                {
                    // PDF
                    string filename = GetAbsoluteFileName(VFileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedVFilBase64.Substring(HttpPostedVFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.FaqPath = Originalpath;
                    }
                }
                #endregion

                #region Schedule PDF
                if (!string.IsNullOrEmpty(HttpPostedSFilBase64) && !string.IsNullOrEmpty(SFileName))
                {
                    // PDF
                    string filename = GetAbsoluteFileName(SFileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedSFilBase64.Substring(HttpPostedSFilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.SchedulePath = Originalpath;
                    }
                }
                #endregion
                #endregion

                // Registration Details Images
                #region Registration Details Upload Images

                #region Description1 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc1FilBase64) && !string.IsNullOrEmpty(Desc1FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc1FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc1FilBase64.Substring(HttpPostedDesc1FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description1PicturePath = Originalpath;
                        model.FileName1 = Desc1FileName;
                    }
                }
                #endregion

                #region Description2 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc2FilBase64) && !string.IsNullOrEmpty(Desc2FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc2FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc2FilBase64.Substring(HttpPostedDesc2FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description2PicturePath = Originalpath;
                        model.FileName2 = Desc2FileName;
                    }
                }
                #endregion

                #region Description3 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc3FilBase64) && !string.IsNullOrEmpty(Desc3FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc3FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc3FilBase64.Substring(HttpPostedDesc3FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description3PicturePath = Originalpath;
                        model.FileName3 = Desc3FileName;
                    }
                }
                #endregion

                #region Description4 Image
                if (!string.IsNullOrEmpty(HttpPostedDesc4FilBase64) && !string.IsNullOrEmpty(Desc4FileName))
                {
                    // Image
                    string filename = GetAbsoluteFileName(Desc4FileName);
                    byte[] fileBytes = Convert.FromBase64String(HttpPostedDesc4FilBase64.Substring(HttpPostedDesc4FilBase64.IndexOf(',') + 1));
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        string Originalpath = "~/UploadFiles/" + model.MeetingName + "/" + filename;
                        DefaultPath = (Server.MapPath(Originalpath));
                        DefaulttargetPath = (Server.MapPath(Originalpath));
                        using (FileStream file = new FileStream(DefaultPath, FileMode.Create, FileAccess.Write))
                            ms.WriteTo(file);
                        model.Description4PicturePath = Originalpath;
                        model.FileName4 = Desc4FileName;
                    }
                }
                #endregion 
                #endregion

                model.CreatedBy = User.Identity.GetUserId();
                bool obj = Commonoperations.EditMettingDeatils(model);
                if (obj == true)
                {
                    TempData["Success"] = "Meeting updated successfully !!";
                }
                else
                {
                    TempData["Error"] = "Error occured while Meeting updation !!";
                }
            }
            Meeting();
            return PartialView("_BindMeetings");
        }

        [HttpGet]
        public ActionResult DeleteMeeting(int MeetingID)
        {
            var model = db.Repository<OCASIA.Meeting.DAL.Meeting>().GetAll().Where(f => f.MeetingID == MeetingID).FirstOrDefault();
            return PartialView("_DeleteMeetingConfirm", model);
        }

        [HttpPost]
        public ActionResult DeleteMeetingConfirm(int MeetingID)
        {
            if (MeetingID != 0)
            {
                var thisuser = db.Repository<OCASIA.Meeting.DAL.Meeting>().GetAll().Where(f => f.MeetingID == MeetingID).FirstOrDefault();
                if (thisuser != null)
                {
                    thisuser.IsActive = false;
                    db.Repository<OCASIA.Meeting.DAL.Meeting>().Update(thisuser);
                    db.SaveChanges();
                    TempData["Success"] = "Meeting (" + thisuser.MeetingName + ") Deleted Successfully";
                }
                else
                {
                    TempData["error"] = "Meeting (" + thisuser.MeetingName + ") record not found!";
                }
            }
            Meeting();
            return PartialView("_BindMeetings");
        }

        private bool ValidateImage(Image image)
        {
            //model.MeetingDescription = Sanitizer.GetSafeHtmlFragment(model.MeetingDescription);
            //model.FaqDescription = Sanitizer.GetSafeHtmlFragment(model.FaqDescription);
            //model.BasicDescription = Sanitizer.GetSafeHtmlFragment(model.BasicDescription);
            //model.Description1 = Sanitizer.GetSafeHtmlFragment(model.Description1);
            //model.Description2 = Sanitizer.GetSafeHtmlFragment(model.Description2);
            //model.Description3 = Sanitizer.GetSafeHtmlFragment(model.Description3);
            //model.Description4 = Sanitizer.GetSafeHtmlFragment(model.Description4);

            if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid) { return true; }

            else if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid) { return true; }

            else if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid) { return true; }

            else if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid) { return true; }

            else if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid) { return true; }

            else if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Icon.Guid) { return true; }

            else
                return false;

        }

        [HttpPost]
        public ActionResult UpdateIsPublishbyMeetingID(int MeetingID, bool IsPublish)
        {
            if (MeetingID != 0)
            {
                using (db = new OCASIAMeetingUOW())
                {
                    var res = db.Repository<OCASIA.Meeting.DAL.Meeting>().GetAllReffByID(a => a.MeetingID == MeetingID).FirstOrDefault();
                    if (res != null)
                    {
                        res.MeetingID = MeetingID;
                        res.IsPublish = IsPublish;
                        db.Repository<OCASIA.Meeting.DAL.Meeting>().Update(res);
                        db.SaveChanges();
                        return Json(IsPublish, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateIsAllowRegistration(int MeetingID, bool IsAllowRegistration)
        {
            if (MeetingID != 0)
            {
                using (db = new OCASIAMeetingUOW())
                {
                    var res1 = db.Repository<OCASIA.Meeting.DAL.Meeting>().GetAllReffByID(a => a.AllowRegistration == true).ToList();
                    if (res1 != null)
                    {
                        foreach (var item in res1)
                        {
                            item.AllowRegistration = false;
                            db.Repository<OCASIA.Meeting.DAL.Meeting>().Update(item);
                            db.SaveChanges();
                        }
                    }
                    var res = db.Repository<OCASIA.Meeting.DAL.Meeting>().GetAllReffByID(a => a.MeetingID == MeetingID).FirstOrDefault();
                    if (res != null)
                    {
                        res.MeetingID = MeetingID;
                        res.AllowRegistration = IsAllowRegistration;
                        db.Repository<OCASIA.Meeting.DAL.Meeting>().Update(res);
                        db.SaveChanges();
                        string retval = IsAllowRegistration == true ? "Yes" : "No";
                        return Json(retval, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}