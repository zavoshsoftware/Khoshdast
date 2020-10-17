using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Khoshdast.Startup))]
namespace Khoshdast

{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
