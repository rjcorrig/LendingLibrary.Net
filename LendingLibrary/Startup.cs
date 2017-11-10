using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LendingLibrary.Startup))]
namespace LendingLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
