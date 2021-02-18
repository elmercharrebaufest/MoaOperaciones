using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(SustitucionMOA.Startup))]

namespace SustitucionMOA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Configuro la autenticación
            ConfigureAuth(app);
            ConfigureHangfire(app);
        }
    }
}
