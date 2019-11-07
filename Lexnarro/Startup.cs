using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lexnarro.Startup))]
namespace Lexnarro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
