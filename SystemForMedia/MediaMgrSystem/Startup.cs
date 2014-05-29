using Microsoft.Owin;
using Owin;
using System.Web.Routing;

[assembly: OwinStartupAttribute(typeof(MediaMgrSystem.Startup))]
namespace MediaMgrSystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}
