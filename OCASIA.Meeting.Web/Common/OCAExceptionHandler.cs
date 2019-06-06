using OCASIA.Meeting.DAL;
using OCASIA.Meeting.DAL.ApplicationModels;
using System;
using System.Web.Mvc;

namespace OCASIA.Meeting.Web
{
    public class OCAExceptionHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception e = filterContext.Exception;
            var model = new HandleErrorInfo(filterContext.Exception, "Account", "Error");
            StaticLogger.Logger.Error(e);
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(model)

            };

        }
    }
   
}