using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancesManagment.Startup))]
namespace FinancesManagment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
