using System;
using Microsoft.Owin;
using Owin;
using Lotereya.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

//[assembly: OwinStartup(typeof(Lotereya.Startup))]

namespace Lotereya
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // настраиваем контекст и менеджер
            app.CreatePerOwinContext<ApplicationContext>(ApplicationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Admin/Login"),
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}