using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OCASIA.Meeting.Registration.Startup))]
namespace OCASIA.Meeting.Registration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
