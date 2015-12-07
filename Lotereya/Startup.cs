using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Lotereya.Startup))]

namespace Lotereya
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}