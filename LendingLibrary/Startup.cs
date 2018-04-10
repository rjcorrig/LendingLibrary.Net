using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartupAttribute(typeof(LendingLibrary.Startup))]
namespace LendingLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            ConfigureAuth(app);

            var config = new HttpConfiguration();
            app.UseWebApi(config);
        }
    }
}
