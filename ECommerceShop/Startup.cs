using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ECommerceShop.Startup))]
namespace ECommerceShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
