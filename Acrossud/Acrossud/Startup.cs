using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Acrossud.Startup))]
namespace Acrossud
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
