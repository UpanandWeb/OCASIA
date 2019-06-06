using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OCASIA.Meeting.Web.Startup))]
namespace OCASIA.Meeting.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
