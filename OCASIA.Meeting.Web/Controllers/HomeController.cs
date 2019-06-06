using System;
using System.Web.Mvc;

namespace OCASIA.Meeting.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.logindate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
    }
}