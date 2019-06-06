using System.ComponentModel.DataAnnotations;

// New namespace imports:

namespace OCASIA.Meeting.DAL
{
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        //public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }


    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email field is required."), StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password field is required."),DataType(DataType.Password),Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password field is required."), DataType(DataType.Password), Display(Name = "Password")]
        public string Passwordhash { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        public string Errormessage { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }

    public class UserModuleMappingModel
    {
        public int UserModuleMappingID { get; set; }
        public int ModuleID { get; set; }

        public virtual string UserId { get; set; }
        public int PermissionLevelID { get; set; }
        public bool IsActive { get; set; }
        public string ModuleName { get; set; }

        public string DisplayModuleName { get; set; }

        public bool MainMenu { get; set; }

        public bool SubMenu { get; set; }

        public int Group { get; set; }


    }

    public class RegisterViewModel
    {

        public string UserId { get; set; }
        public string Datecreated { get; set; }
        public string RoleName { get; set; }      

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Chnagepassword")]
        public string Chnagepassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Chnagepassword")]
        public string ConfirmChnagepassword { get; set; }


        [Required]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public int RoleCustomID { get; set; }        
        public int CountryID { get; set; }

    }


    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email field is required."), StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string UserId { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email field is required."), StringLength(50), DataType(DataType.EmailAddress), Display(Name = "Email"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }
    }
}