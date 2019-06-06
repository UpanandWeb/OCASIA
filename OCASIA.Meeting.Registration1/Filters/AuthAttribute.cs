using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace OCASIA.Meeting.Registration.Filters
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
                        
                        context.Result =                            
                            new RedirectToRouteResult("NotPermitted",
                      new System.Web.Routing.RouteValueDictionary{
                        {"controller", "Participant"},
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
                    context.Result = new RedirectToRouteResult("Login",
                       new System.Web.Routing.RouteValueDictionary{
                        {"controller", "Participant"},
                        {"action", "Login"}
                       });
                }
            }
        }
    }
}