using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace OCASIA.Meeting.Web.Filters
{
    public class AuthAttribute
    {
        public class SuperAdminAuthAttribute : FilterAttribute, IAuthenticationFilter
        {
            public void OnAuthentication(AuthenticationContext context)
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    string CurrentRole = Helper.CurrentUserRole();
                    if (CurrentRole != null)
                    {
                        // do proceed    
                    }
                    else
                    {
                        context.Result = new RedirectToRouteResult("Default",
                      new System.Web.Routing.RouteValueDictionary{
                        {"controller", "Home"},
                        {"action", "PageNotFound"}
                      });
                    }
                }
                else
                {
                    context.Result = new HttpUnauthorizedResult(); // mark unauthorized
                }
            }

            public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
            {
                if (context.Result is HttpUnauthorizedResult)
                {
                    context.Result = new RedirectToRouteResult("Default",
                       new System.Web.Routing.RouteValueDictionary{
                        {"controller", "Account"},
                        {"action", "Login"}
                       });
                }
            }
        }
    }
}