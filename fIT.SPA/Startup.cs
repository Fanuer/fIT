using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fIT.SPA.Startup))]
namespace fIT.SPA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
