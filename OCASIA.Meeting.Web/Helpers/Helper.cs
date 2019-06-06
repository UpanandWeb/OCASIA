using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Text;
using System.Net;
using OCASIA.Meeting.DAL;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Security.Cryptography;

namespace OCASIA.Meeting.Web
{
    public class Helper
    {
        public static string UI_DateFormt { get; set; } = "dd MMM yyyy";

        public static string[] DB_DateFormat = new string[] { "MM/dd/yyyy" };

        static DateTime dob = new DateTime(2002, 10, 22);
        static string[] dateFormats = dob.GetDateTimeFormats();
        public static bool GetDate(string dateString, out DateTime convertedDate)
        {
            try
            {
                if (string.IsNullOrEmpty(dateString))
                {
                    convertedDate = DateTime.MaxValue;
                    return false;
                }
                return DateTime.TryParseExact(dateString, DB_DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate);
            }
            catch { throw; }
        }


        public static string AccessToken;
        public static String GetUserPlatform()
        {
            var ua = HttpContext.Current.Request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return HttpContext.Current.Request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public static String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Upload Image
        public static bool GeneratePictureGalleryThumbnails(int Width, int Height, Stream sourcePath, string targetPath)
        {
            using (var image = Image.FromStream(sourcePath))
            {
                var thumbnailImg = new Bitmap(Width, Height);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, Width, Height);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbnailImg.Save(targetPath, image.RawFormat);
                return true;
            }

        }

        private static bool ValidateImage(Image image)
        {
            return false;
        }


        #endregion

        #region Upload Image
        public static bool GenerateThumbnails(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = Image.FromStream(sourcePath))
            {
                // can given width of image as we want
                var newWidth = (int)(image.Width * scaleFactor);
                // can given height of image as we want
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbnailImg.Save(targetPath, image.RawFormat);
                return true;
            }

        }
        #endregion      

        public static string GetIP()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

        public static string CurrentUserRole()
        {
            OCASIAMeetingUOW db = new OCASIAMeetingUOW();

            string UserId = HttpContext.Current.User.Identity.GetUserId();
            var CurrentRole = db.Repository<ApplicationUser>().GetAll().Where(a => a.Id == UserId).Include(a => a.RoleCustoms).Select(a => a.RoleCustoms.RoleName).FirstOrDefault();
            if (CurrentRole != null)
            {
                return CurrentRole;
            }
            return null;
        }

        public static string CurrentUserID()
        {
            string UserId = HttpContext.Current.User.Identity.GetUserId();
            return UserId;
        }

        public static int CurrentUserRoleID()
        {
            OCASIAMeetingUOW db = new OCASIAMeetingUOW();

            string UserId = HttpContext.Current.User.Identity.GetUserId();
            var RoleCustomID = db.Repository<ApplicationUser>().GetAll().Where(a => a.Id == UserId).Include(a => a.RoleCustoms).Select(a => a.RoleCustoms.RoleCustomID).FirstOrDefault();
            if (RoleCustomID != 0)
            {
                return RoleCustomID;
            }
            return 0;
        }

        public static int UserRoleID()
        {
            OCASIAMeetingUOW db = new OCASIAMeetingUOW();

            var RoleCustomID = db.Repository<RoleCustom>().GetAll().Where(a => a.RoleName == "User").Select(a => a.RoleCustomID).FirstOrDefault();
            if (RoleCustomID != 0)
            {
                return RoleCustomID;
            }
            return 0;
        }

        public static int AdminRoleID()
        {
            OCASIAMeetingUOW db = new OCASIAMeetingUOW();

            var RoleCustomID = db.Repository<RoleCustom>().GetAll().Where(a => a.RoleName == "Admin").Select(a => a.RoleCustomID).FirstOrDefault();
            if (RoleCustomID != 0)
            {
                return RoleCustomID;
            }
            return 0;
        }

        public static string Encrypt(string plainText)
        {
            string EncryptionKey = "012345ABCDEF";
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainText;

        }
    }
}